module rookpromptapitest.Unit.TestJWTTokenService

open System
open Xunit

open rookpromptapitest

open rookpromptapi.Models
open rookpromptapi.Services
open rookpromptapi.Services.Impl
open rookpromptapi.Services.Memory
open Microsoft.IdentityModel.Tokens

let testUser: User = {
    Id = "111"
    Name = "bob"
    Email = "bob@foo.com"
    Roles = ["normy"]
    Created = DateTime.UtcNow
    Updated = DateTime.UtcNow
}

[<Fact>]
let TestTokenService () =
    async {
        let secretService: ISecretService = new MemorySecretService()
        let tokenService: ITokenService = new JWTTokenService(
            secretService,
            "userLoginTokenSecret",
            24.0,
            24.0 * 365.0
        )
    
        let! jwt = tokenService.Create testUser
        let! validatedUser = tokenService.Validate jwt

        Assert.Equal(testUser.Id, validatedUser.Id)
        Assert.Equal<string list>(testUser.Roles, validatedUser.Roles)
    } |> Async.StartAsTask

[<Fact>]
let TestTokenServiceDifferentSecret () =
    async {
        let secretService: ISecretService = new MemorySecretService()
        let tokenService: ITokenService = new JWTTokenService(
            secretService,
            "userLoginTokenSecret",
            24.0,
            24.0 * 365.0
        )

        let! jwt = tokenService.Create testUser

        // Delete the secret forcing a new one
        let! r = secretService.Delete "userLoginTokenSecret"

        try
            let! validatedUser = tokenService.Validate jwt
            Assert.True(false, "Expected failure.")
        with
            | :? SecurityTokenException -> ()

    } |> Async.StartAsTask