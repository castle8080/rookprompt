namespace rookpromptapi.Services.Impl

open System
open System.Threading.Tasks

open System.Security.Cryptography 


open rookpromptapi.Services
open rookpromptapi.Models

type HMACSecretHashService(saltBytes: int, hmacFactory: byte[] -> HMAC) =

    let getSalt () =
        use rng = RandomNumberGenerator.Create()
        let salt : byte[] = Array.zeroCreate saltBytes
        rng.GetBytes(salt)
        salt

    let getHashedRaw (salt: byte[]) (secret: byte[]) =
        use hmac = hmacFactory salt
        hmac.ComputeHash secret

    let getHashValue (salt: byte[]) (secret: byte[]) =
        let hashValue = getHashedRaw salt secret
        let hashWSalt = Array.concat [salt; hashValue]
        Convert.ToBase64String hashWSalt

    let parseHashedValue (hashedValue: string) =
        let rawBytes = Convert.FromBase64String hashedValue
        if Array.length rawBytes <= saltBytes then
            raise (new ArgumentException("Invalid size for hashed secret."))
        (rawBytes[0..(saltBytes-1)], rawBytes[saltBytes..])

    interface ISecretHashService with
        member this.Hash (secret: byte[]): string =
            getHashValue (getSalt()) secret

        member this.Verify (secret: byte[]) (hashedSecret: string): bool =
            let (salt, previousHashedValue) = parseHashedValue hashedSecret
            let verifyHashedValue = getHashedRaw salt secret

            // Note: you should compare all values (timing attack)
            let mutable error = false
            for i in 0..(verifyHashedValue.Length-1) do
                if verifyHashedValue[i] <> previousHashedValue[i] then
                    error <- true
             
            not error

module HMACSecretHashService =
    let CreateWithSHA256 () =
        let hmacFunc (key: byte[]) = new HMACSHA256(key) :> HMAC
        new HMACSecretHashService(64, hmacFunc) :> ISecretHashService