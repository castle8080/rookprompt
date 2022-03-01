namespace rookpromptapi.Services.Mongo

open System

open MongoDB.Bson

// Utilities to help create bson document.
module BsonX =
    let bobj (elements: (string * Object) list) =
        Map<string, Object>(elements)

    let bdoc (elements: (string * Object) list) =
        new BsonDocument(bobj elements)