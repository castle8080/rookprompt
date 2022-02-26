module rookpromptapitest.Integration.TestDBCore

open System
open Xunit

open rookpromptapitest
open rookpromptapi.Services

open MongoDB.Bson
open MongoDB.Driver

[<Fact>]
let TestDBConnectivity () =
    let dbClient = TestServices.GetService<MongoClient>()
    Assert.NotNull(dbClient)
