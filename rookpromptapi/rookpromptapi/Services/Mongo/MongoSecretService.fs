namespace rookpromptapi.Services.Mongo

open System
open System.Threading.Tasks

open MongoDB.Bson
open MongoDB.Driver

open rookpromptapi.Services
open rookpromptapi.Models

open rookpromptapi.Services.Mongo.BsonX
open rookpromptapi.Services.Mongo.MongoX

type MongoSecretService(mongoClient: MongoClient, databaseName: string) =

    let getDb () =
        mongoClient.GetDatabase(databaseName)

    let getSecrets () =
        getDb().GetCollection<BsonDocument>("secrets")

    let fromBson (doc: BsonDocument): Secret =
        {
            Id = doc["_id"].ToString()
            Secret = doc["secret"].AsString
            Description = doc["description"].AsString
            Expires = doc["expires"].AsBsonDateTime.ToUniversalTime()
            Created = doc["created"].AsBsonDateTime.ToUniversalTime()
            Updated = doc["updated"].AsBsonDateTime.ToUniversalTime()
        }

    let fromNullableBson (doc: BsonDocument): Secret option =
        doc |> Option.ofObj |> Option.map fromBson

    let toBson (s: Secret): BsonDocument =
        bdoc [
            ("_id", s.Id)
            ("secret", s.Secret)
            ("description", s.Description)
            ("expires", s.Expires)
            ("created", s.Created)
            ("updated", s.Updated)
        ]

    interface ISecretService with
    
        member this.FindById(id: string): Secret option Async =
            getSecrets() |> MongoX.findByIdAsync id fromBson

        member this.Delete (id: string): bool Async =
            getSecrets() |> MongoX.deleteByIdAsync id

        member this.List(): Secret list Async =
            getSecrets() |> MongoX.findAsync (bdoc[]) fromBson 

        member this.Update(secret: Secret): Secret Async =
            async {
                let! foundSecret = (this :> ISecretService).FindById(secret.Id)
                match foundSecret with
                    | None ->
                        return (raise (DatabaseOperationFailure $"Secret already exists: {secret.Id}"))
                    | Some(foundSecret) ->
                        let newSecret = {
                            secret with
                                Created = foundSecret.Created
                                Updated = DateTime.UtcNow
                        }
                        let! r = getSecrets() |> MongoX.replaceAsync (toBson newSecret)
                        match r with
                            | false ->
                                return raise (DatabaseOperationFailure $"Failed to update secret: {secret.Id}")
                            | true ->
                                return newSecret
            }

        member this.Create(secret: Secret): Secret Async =
            async {
                return! getSecrets() |> MongoX.insertAsync (toBson secret) fromBson 
            }
