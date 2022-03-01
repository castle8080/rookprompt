namespace rookpromptapi.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open rookpromptapi
open rookpromptapi.Models
open rookpromptapi.Services
open rookpromptapi.Services.SecretHashServiceX

module UserController =
    // A separate request type is used to create the user to double check email
    // and to accept credentials on creation.
    type UserCreationRequest = {
        Name : string
        Email : string
        EmailVerify : string
        Password : string
        PasswordVerify : string
    }

    type LoginRequest = {
        Email : string
        Password : string
    }

    type LoginResponse = {
        Token: string
    }

[<ApiController>]
type UserController(
    logger : ILogger<UserController>,
    userService: IUserService,
    userCredentialService: IUserCredentialService,
    secretHashService: ISecretHashService) =
    inherit ControllerBase()

    [<HttpGet>]
    [<Route("api/users")>]
    member this.List() =
        userService.List() |> Async.StartAsTask

    [<HttpPost>]
    [<Route("api/users")>]
    member this.Post([<FromBodyAttribute>] userRequest: UserController.UserCreationRequest) =
        
        // Basic validity check.
        // Add better checks.
        match userRequest with
            | { Email = null } | { Email = "" } | { EmailVerify = null } | { EmailVerify = "" } ->
                raise (InputValdationError "Missing email.")
            | { Password = null } | { Password = "" } | { PasswordVerify = null } | { PasswordVerify = "" } ->
                raise (InputValdationError "Missing password.")
            | { Name = null } | { Name = "" } ->
                raise (InputValdationError "Missing name.")
            | _ when userRequest.Email <> userRequest.EmailVerify ->
                raise (InputValdationError "Email does not match.")
            | _ when userRequest.Password <> userRequest.PasswordVerify ->
                raise (InputValdationError "Passwords do not match.")
            | _ ->
                ()

        let user: User = {
            Id = ""
            Name = userRequest.Name
            Email = userRequest.Email
            Roles = []
            Created = DateTime.UtcNow
            Updated = DateTime.UtcNow
        }

        let hashedSecret = secretHashService.HashString userRequest.Password

        async {
            logger.LogInformation($"Received new user: {user}")
            let! user = userService.Create user
            logger.LogInformation($"User created {user}")
            
            let credential : UserCredential = {
                Id = user.Id
                Secret = hashedSecret
                Created = DateTime.UtcNow
                Updated = DateTime.UtcNow
            }
            
            let! ucResult = userCredentialService.Save credential
            logger.LogInformation($"Saved credentials for user.")

            return user
        }

    [<HttpPost>]
    [<Route("api/login")>]
    member this.Login([<FromBodyAttribute>] loginRequest: UserController.LoginRequest): UserController.LoginResponse Async =
        let getUser () =
            async {
                match! userService.FindByEmail loginRequest.Email with
                    | None ->
                        return (raise (LoginError $"Could not login user {loginRequest.Email}"))
                    | Some(user) ->
                        return user
            }

        let getUserCredential (user: User) =
            async {
                match! userCredentialService.FindByUserId user.Id with
                    | None ->
                        return (raise (LoginError $"Could not login user {loginRequest.Email}"))
                    | Some(userCredential) ->
                        return userCredential 
            }

        async {
            let! user = getUser()
            let! userCredential = getUserCredential user
            let authenticated = secretHashService.VerifyString loginRequest.Password userCredential.Secret

            if not authenticated then
                return (raise (LoginError $"Could not login user {loginRequest.Email}"))

            return {
                // Todo: create a real token.
                Token = "you logged in"
            }
        }