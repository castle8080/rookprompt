namespace rookpromptapi.Services.Mongo

open System
open System.Threading.Tasks

open MongoDB.Bson
open MongoDB.Driver

open rookpromptapi.Services
open rookpromptapi.Models

open rookpromptapi.Services.Mongo.BsonX
open rookpromptapi.Services.Mongo

type MongoPromptService(mongoClient: MongoClient, databaseName: string) =

    let fromBson (doc: BsonDocument) : Prompt =
        {
            Id = (doc["_id"].AsObjectId).ToString()
            Prompt = doc["prompt"].AsString
            Created = doc["created"].AsBsonDateTime.ToUniversalTime()
            Updated = doc["updated"].AsBsonDateTime.ToUniversalTime()
        }

    let fromNullableBson (doc: BsonDocument): Prompt option =
        doc |> Option.ofObj |> Option.map fromBson

    let toBson (p: Prompt): BsonDocument =
        let doc = bdoc [
            ("prompt", p.Prompt)
            ("created", p.Created)
            ("updated", p.Updated)
        ]
        match p.Id with
            | "" | null -> ()
            | _ -> doc.Add("_id", new ObjectId(p.Id)) |> ignore
        doc

    let getDb () =
        mongoClient.GetDatabase(databaseName)

    let getPrompts () =
        getDb().GetCollection<BsonDocument>("prompts")

    let promptEq (prompt: string) =
        bdoc [("prompt", prompt)]

    let insertAsync (p: Prompt) =
        getPrompts() |> MongoX.insertAsync (toBson p) fromBson

    let updateAsync (p: Prompt) =
        getPrompts() |> MongoX.replaceAsync (toBson p)

    interface IPromptService with

        member this.List(): Prompt list Async =
            getPrompts() |> MongoX.findAsync (bdoc[]) fromBson 

        member this.Save(p: Prompt): Prompt Async =
            async {
                match! (this :> IPromptService).FindByPrompt(p.Prompt) with
                    | Some(existingP) ->
                        let newP = {
                            p with
                                Id = existingP.Id
                                Created = existingP.Created
                                Updated = DateTime.UtcNow
                        }
                        let! updated = updateAsync newP
                        match updated with
                            | false ->
                                return raise (DatabaseOperationFailure $"Failed to update prompt: {newP.Id}")
                            | true ->
                                return newP
                    | None ->
                        let p = {
                            p with
                                Id = ""
                                Created = DateTime.UtcNow
                                Updated = DateTime.UtcNow
                        }
                        return! insertAsync p
            }

        member this.DeleteById (id: string): bool Async =
            getPrompts() |> MongoX.deleteByIdAsync (new ObjectId(id))

        member this.FindById (id: string) =
            getPrompts() |> MongoX.findByIdAsync (new ObjectId(id)) fromBson

        member this.FindByPrompt (prompt: string) =
            getPrompts() |> MongoX.findOneAsync (promptEq prompt) fromBson
            
        member this.SampleOne (): Prompt option Async =
            async {
                let! results =
                    getPrompts()
                        .Aggregate()
                        .Sample(1)
                        .ToListAsync()
                        |> Async.AwaitTask
                return results |> Seq.tryHead |> Option.map fromBson
            }