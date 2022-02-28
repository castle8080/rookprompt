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

    let findOneAsync (filter: BsonDocument) =
        getPrompts() |> MongoX.findOneAsync filter fromBson

    let findAsync (filter: BsonDocument) =
        getPrompts() |> MongoX.findAsync filter fromBson

    interface IPromptService with

        member this.List(): Prompt list Async =
            findAsync bdoc[]

        member this.Save(p: Prompt): Prompt Async =
            async {
                match! (this :> IPromptService).FindByPrompt(p.Prompt) with
                    | Some(p) ->
                        // There is nothing to update yet.
                        // Could update the timestamp though.
                        return p
                    | None ->
                        return! insertAsync p
            }

        member this.DeleteById (id: string): bool Async =
            getPrompts() |> MongoX.deleteByIdAsync (new ObjectId(id))

        member this.FindById (id: string) =
            getPrompts() |> MongoX.findByIdAsync (new ObjectId(id)) fromBson

        member this.FindByPrompt (prompt: string) =
            findOneAsync (promptEq prompt)
            
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