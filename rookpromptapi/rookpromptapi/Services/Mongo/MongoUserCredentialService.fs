namespace rookpromptapi.Services.Mongo

open System
open System.Threading.Tasks

open MongoDB.Bson
open MongoDB.Driver

open rookpromptapi.Services
open rookpromptapi.Models

open rookpromptapi.Services.Mongo.BsonX
open rookpromptapi.Services.Mongo.MongoX

type MongoUserCredentialService(mongoClient: MongoClient, databaseName: string) =

    let getDb () =
        mongoClient.GetDatabase(databaseName)

    let getUserCredentials () =
        getDb().GetCollection<BsonDocument>("userCredentials")

    let fromBson (doc: BsonDocument): UserCredential =
        {
            Id = (doc["_id"].AsObjectId).ToString()
            Secret = doc["secret"].AsString
            Created = doc["created"].AsBsonDateTime.ToUniversalTime()
            Updated = doc["updated"].AsBsonDateTime.ToUniversalTime()
        }

    let fromNullableBson (doc: BsonDocument): UserCredential option =
        doc |> Option.ofObj |> Option.map fromBson

    let toBson (uc: UserCredential): BsonDocument =
        bdoc [
            ("_id", new ObjectId(uc.Id))
            ("secret", uc.Secret)
            ("created", uc.Created)
            ("updated", uc.Updated)
        ]

    interface IUserCredentialService with

        member this.FindByUserId(userId: string): UserCredential option Async =
            getUserCredentials() |> MongoX.findByIdAsync (new ObjectId(userId)) fromBson
    
        member this.Save (uc: UserCredential) =
            async {
                let! foundUc = (this :> IUserCredentialService).FindByUserId(uc.Id)
                match foundUc with
                    | None ->
                        let newUc = {
                            uc with
                                Created = DateTime.UtcNow
                                Updated = DateTime.UtcNow
                        }
                        return! getUserCredentials() |> MongoX.insertAsync (toBson newUc) fromBson
                    | Some(foundUc) ->
                        let newUc = {
                            uc with
                                Created = foundUc.Created
                                Updated = DateTime.UtcNow
                        }
                        let! r = getUserCredentials() |> MongoX.replaceAsync (toBson newUc)
                        match r with
                            | false ->
                                return (raise (DatabaseOperationFailure $"Could not update credentials for user: {newUc.Id}"))
                            | true ->
                                return newUc
            }