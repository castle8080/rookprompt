namespace rookpromptapi.Services.Mongo

open System
open System.Threading.Tasks

open MongoDB.Bson
open MongoDB.Driver

open rookpromptapi.Services
open rookpromptapi.Models

open rookpromptapi.Services.Mongo.BsonX
open rookpromptapi.Services.Mongo.MongoX

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
        if p.Id <> "" then
            doc.Add("_id", new ObjectId(p.Id)) |> ignore
        doc

    let getDb () =
        mongoClient.GetDatabase(databaseName)

    let getPrompts () =
        getDb().GetCollection<BsonDocument>("prompts")

    let promptEq (prompt: string) =
        bdoc [("prompt", prompt)]

    let idEq (id: string) =
        bdoc [("_id", new ObjectId(id))]

    let findOneAsync (filter: BsonDocument) =
        task {
            let! cur = getPrompts().FindAsync(filter)
            let! doc = cur.FirstOrDefaultAsync()
            return doc |> Option.ofObj |> Option.map fromBson
        }

    let findAsync (filter: BsonDocument) =
        task {
            let! cur = getPrompts().FindAsync(filter)
            let! docs = cur.ToListAsync()
            return docs |> Seq.map fromBson |> List.ofSeq
        }

    let insertAsync (p: Prompt) =
        task {
            let doc = toBson p
            do! getPrompts().InsertOneAsync(doc)
            return fromBson doc
        }

    interface IPromptService with

        member this.List(): Prompt list Task =
            findAsync (new BsonDocument())

        member this.Save(p: Prompt): Prompt Task =
            task {
                match! (this :> IPromptService).FindByPrompt(p.Prompt) with
                    | Some(p) ->
                        // There is nothing to update yet.
                        // Could update the timestamp though.
                        return p
                    | None ->
                        return! insertAsync p
            }

        member this.DeleteById (id: string): bool Task =
            task {
                let! r = getPrompts().FindOneAndDeleteAsync(idEq id)
                return r |> Option.ofObj |> Option.isSome
            }

        member this.FindById (id: string) =
            findOneAsync (idEq id)

        member this.FindByPrompt (prompt: string) =
            findOneAsync (promptEq prompt)
            
        member this.SampleOne (): Prompt option Task =
            task {
                let! results =
                    getPrompts()
                        .Aggregate()
                        .Sample(1)
                        .ToListAsync()
                return results |> Seq.tryHead |> Option.map fromBson
            }