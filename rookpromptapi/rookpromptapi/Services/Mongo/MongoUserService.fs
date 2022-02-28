namespace rookpromptapi.Services.Mongo

open System
open System.Threading.Tasks

open MongoDB.Bson
open MongoDB.Driver

open rookpromptapi.Services
open rookpromptapi.Models

open rookpromptapi.Services.Mongo.BsonX
open rookpromptapi.Services.Mongo.MongoX

type MongoUserService(mongoClient: MongoClient, databaseName: string) =

    let getDb () =
        mongoClient.GetDatabase(databaseName)

    let getUsers () =
        getDb().GetCollection<BsonDocument>("users")

    let fromBson (doc: BsonDocument): User =
        {
            Id = (doc["_id"].AsObjectId).ToString()
            Name = doc["name"].AsString
            Email = doc["email"].AsString
            Created = doc["created"].AsBsonDateTime.ToUniversalTime()
            Updated = doc["updated"].AsBsonDateTime.ToUniversalTime()
            Roles =
                match doc["roles"] with
                    | :? BsonArray as a ->
                        a.Values |> Seq.map (fun x -> x.ToString()) |> List.ofSeq
                    | _ -> []
        }

    let fromNullableBson (doc: BsonDocument): User option =
        doc |> Option.ofObj |> Option.map fromBson

    let toBson (u: User): BsonDocument =
        let doc = bdoc [
            ("name", u.Name)
            ("email", u.Email)
            ("created", u.Created)
            ("updated", u.Updated)
            ("roles", u.Roles)
        ]
        match u.Id with
            | "" | null -> ()
            | _ -> doc.Add("_id", new ObjectId(u.Id)) |> ignore
        doc

    let emailEq (email: string) =
        bdoc [("email", email)]

    interface IUserService with
        member this.List(): User list Async =
            getUsers() |> MongoX.findAsync (bdoc[]) fromBson 

        member this.Update(user: User): User Async =
            async {
                let! foundUser = (this :> IUserService).FindByEmail(user.Email)
                match foundUser with
                    | None ->
                        return raise (UserNotFound $"User not found: {user.Email}")
                    | Some(foundUser) ->
                        let newUser = {
                            user with
                                Id = foundUser.Id
                                Created = foundUser.Created
                                Updated = DateTime.UtcNow
                        }
                        let! r = getUsers() |> MongoX.replaceAsync (toBson newUser)
                        match r with
                            | false ->
                                return raise (DatabaseOperationFailure $"Failed to update user: {newUser.Id}")
                            | true ->
                                return newUser
            }

        member this.FindByEmail(email: string): User option Async =
            getUsers() |> MongoX.findOneAsync (emailEq email) fromBson

        member this.FindById(id: string): User option Async =
            getUsers() |> MongoX.findByIdAsync (new ObjectId(id)) fromBson

        member this.Create(user: User): User Async =
            async {
                match! (this :> IUserService).FindByEmail(user.Email) with
                    | Some(foundUser) ->
                        return raise (UserExists $"A user with email {user.Email} already exists.")
                    | None ->
                        let user = {
                            user with
                                Id = ""
                                Created = DateTime.UtcNow
                                Updated = DateTime.UtcNow
                        }
                        return! getUsers() |> MongoX.insertAsync (toBson user) fromBson  
            }
