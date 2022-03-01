module rookpromptapitest.Unit.TestHMACSecretHashService

open System
open Xunit

open rookpromptapitest
open rookpromptapi.Services.SecretHashServiceX
open rookpromptapi.Services.Impl

[<Fact>]
let TestSecretHashing () =
    let hashService = HMACSecretHashService.CreateWithSHA256()

    let h1 = hashService.HashString "my_secret"
    let h2 = hashService.HashString "my_secret"
    let h3 = hashService.HashString "other_secret"

    h1 <> h2 |> Assert.True
    h1 <> h3 |> Assert.True

    hashService.VerifyString "my_secret" h1 |> Assert.True
    hashService.VerifyString "my_secret" h2 |> Assert.True
    hashService.VerifyString "other_secret" h3 |> Assert.True
    
    hashService.VerifyString "other_secret" h1 |> Assert.False
    hashService.VerifyString "my_secret" h3 |> Assert.False