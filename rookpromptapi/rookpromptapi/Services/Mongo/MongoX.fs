namespace rookpromptapi.Services.Mongo

open System
open System.Runtime.CompilerServices

open MongoDB.Driver
open MongoDB.Bson

open rookpromptapi.Services.Mongo.BsonX

module MongoX =
    [<Extension>]
    type MongoX =

        [<Extension>]
        static member Sample(this: IAggregateFluent<'a>, size: int) =
            this
                .AppendStage(new BsonDocumentPipelineStageDefinition<'a, 'a>(
                    bdoc [
                        ("$sample", bobj [("size", 1)])
                    ] 
                ))