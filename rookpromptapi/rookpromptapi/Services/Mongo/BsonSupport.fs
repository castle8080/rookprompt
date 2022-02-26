namespace rookpromptapi.Services.Mongo

open System

open MongoDB.Bson

module BsonSupport =
    let bobj (elements: (string * Object) list) =
        Map<string, Object>(elements)

    let bdoc (elements: (string * Object) list) =
        new BsonDocument(bobj elements)