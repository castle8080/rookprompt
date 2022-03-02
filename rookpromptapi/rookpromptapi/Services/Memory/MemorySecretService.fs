namespace rookpromptapi.Services.Memory

open System
open System.Threading.Tasks
open System.Collections.Generic

open rookpromptapi.Services
open rookpromptapi.Models

// Memmory based implementation useful for testing.
type MemorySecretService() =

    let mutable secrets: Map<string, Secret> = Map.empty

    interface ISecretService with
    
        member this.FindById(id: string): Secret option Async =
            async {
                return (secrets |> Map.tryFind id)
            }

        
        member this.Delete(id: string): bool Async =
            async {
                match secrets.ContainsKey id with
                    | true ->
                        secrets <- secrets |> Map.remove id
                        return true
                    | false ->
                        return false
            }

        member this.List(): Secret list Async =
            async {
                return (secrets |> Map.values |> List.ofSeq)
            } 

        member this.Update(secret: Secret): Secret Async =
            async {
                let! foundSecret = (this :> ISecretService).FindById(secret.Id)
                match foundSecret with
                    | None ->
                        return (raise (DatabaseOperationFailure $"Secret does not exists: {secret.Id}"))
                    | Some(foundSecret) ->
                        let newSecret = {
                            secret with
                                Created = foundSecret.Created
                                Updated = DateTime.UtcNow
                        }
                        secrets <- secrets |> Map.add secret.Id secret
                        return newSecret
            }

        member this.Create(secret: Secret): Secret Async =
            async {
                let! foundSecret = (this :> ISecretService).FindById(secret.Id)
                match foundSecret with
                    | Some(foundSecret) ->
                        return (raise (DatabaseOperationFailure $"Secret already exists: {secret.Id}"))
                    | None ->
                        let newSecret = {
                            secret with
                                Created = DateTime.UtcNow
                                Updated = DateTime.UtcNow
                        }
                        secrets <- secrets |> Map.add secret.Id secret
                        return newSecret
            }
