namespace rookpromptapi.Services.Mongo

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices

open MongoDB.Driver
open MongoDB.Bson

open rookpromptapi.Services.Mongo.BsonX

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

// Utility functions for common operations on Mongo.
module MongoX =

    let findOneAsync (filter: BsonDocument) (fromBson: BsonDocument -> 'a) (c: IMongoCollection<BsonDocument>) =
        async {
            let! cur = c.FindAsync(filter) |> Async.AwaitTask
            let! doc = cur.FirstOrDefaultAsync() |> Async.AwaitTask
            return doc |> Option.ofObj |> Option.map fromBson
        }

    let findAsync (filter: BsonDocument) (fromBson: BsonDocument -> 'a) (c: IMongoCollection<BsonDocument>) =
        async {
            let! cur = c.FindAsync(filter) |> Async.AwaitTask
            let! docs = cur.ToListAsync() |> Async.AwaitTask
            return docs |> Seq.map fromBson |> List.ofSeq
        }

    let findByIdAsync (id: 'a) (fromBson: BsonDocument -> 'b) (c: IMongoCollection<BsonDocument>) =
        findOneAsync (bdoc[("id", id :> Object)]) fromBson c

    let insertAsync (doc: BsonDocument) (fromBson: BsonDocument -> 'a) (c: IMongoCollection<BsonDocument>) =
        async {
            do! c.InsertOneAsync(doc) |> Async.AwaitTask
            return fromBson doc
        }

    let deleteByIdAsync (id: 'a) (c: IMongoCollection<'b>): bool Async =
        async {
            let! r = c.FindOneAndDeleteAsync(bdoc[("_id", id :> Object)]) |> Async.AwaitTask
            return r |> Option.ofObj |> Option.isSome
        }

    let replaceAsync (doc: BsonDocument) (c: IMongoCollection<BsonDocument>) =
        let filter = bdoc[("_id", doc["_id"])]
        async {
            let! r = c.ReplaceOneAsync(filter, doc) |> Async.AwaitTask
            return r.IsAcknowledged
        }