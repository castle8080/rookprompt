namespace rookpromptapi.Services.Mongo

open System
open System.Runtime.CompilerServices

open MongoDB.Driver
open MongoDB.Bson

open rookpromptapi.Services.Mongo.BsonX

module MongoX =
    [<Extension>]
    type MongoX =

        /// <summary>
        /// Extends a document aggregation pipeline to include a sample method.
        /// </summary>
        [<Extension>]
        static member Sample(this: IAggregateFluent<'a>, size: int) =
            this
                .AppendStage(new BsonDocumentPipelineStageDefinition<'a, 'a>(
                    bdoc [
                        ("$sample", bobj [("size", size)])
                    ] 
                ))