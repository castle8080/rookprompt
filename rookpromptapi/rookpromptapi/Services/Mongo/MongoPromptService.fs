namespace rookpromptapi.Services.Mongo

open System
open rookpromptapi.Services
open rookpromptapi.Models

open MongoDB.Bson
open MongoDB.Driver

type MongoPromptService(mongoClient: MongoClient, databaseName: string) =

    let fromBson (doc: BsonDocument) : Prompt =
        {
            Id = doc["id"].AsInt32
            Prompt = doc["prompt"].AsString
            Created = doc["created"].AsBsonTimestamp.ToUniversalTime()
            Updated = doc["updated"].AsBsonTimestamp.ToUniversalTime()
        }

    interface IPromptService with
        member this.List(): Prompt list =
            let db = mongoClient.GetDatabase(databaseName)
            db
                .GetCollection<BsonDocument>("prompts")
                .Find(new BsonDocument())
                .ToEnumerable()
                |> Seq.map fromBson
                |> List.ofSeq

        member this.Save(p: Prompt): Prompt = 
            raise (System.NotImplementedException())
        

