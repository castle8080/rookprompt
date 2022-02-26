﻿namespace rookpromptapi.Services.Mongo

open System

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

    let findOne (filter: BsonDocument) =
        getPrompts()
            .Find(filter)
            .FirstOrDefault()
            |> Option.ofObj
            |> Option.map fromBson

    let find (filter: BsonDocument) =
        getPrompts()
            .Find(filter)
            .ToEnumerable()
            |> Seq.map fromBson
            |> List.ofSeq

    let insert (p: Prompt) =
        let doc = toBson p
        getPrompts().InsertOne(doc)
        fromBson doc

    interface IPromptService with

        member this.List(): Prompt list =
            find (new BsonDocument())

        member this.Save(p: Prompt): Prompt =
            match (this :> IPromptService).FindByPrompt(p.Prompt) with
                | Some(p) ->
                    // There is nothing to update yet.
                    // Could update the timestamp though.
                    p
                | None ->
                    insert p

        member this.DeleteById (id: string): bool =
            getPrompts()
                .FindOneAndDelete(idEq id)
                |> Option.ofObj
                |> Option.isSome

        member this.FindById (id: string) =
            findOne (idEq id)

        member this.FindByPrompt (prompt: string) =
            findOne (promptEq prompt)
            
        member this.SampleOne (): Prompt option =
            getPrompts()
                .Aggregate()
                .Sample(1)
                .ToEnumerable()
                |> Seq.tryHead
                |> Option.map fromBson