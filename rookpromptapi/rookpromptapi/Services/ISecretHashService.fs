namespace rookpromptapi.Services

open System
open System.Text

/// <summary>
/// Service for hashing a secret.
/// </summary>
type ISecretHashService =
    abstract member Hash : byte[] -> string

    abstract member Verify : byte[] -> string -> bool

module SecretHashServiceX =
    type ISecretHashService with
        member this.HashString (secret: string): string =
            Encoding.UTF8.GetBytes(secret) |> this.Hash

        member this.VerifyString (secret: string) (hashedValue: string): bool =
            this.Verify (Encoding.UTF8.GetBytes secret) hashedValue