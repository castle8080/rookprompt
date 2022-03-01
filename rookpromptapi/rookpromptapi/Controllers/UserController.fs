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

[<ApiController>]
type UserController(
    logger : ILogger<UserController>,
    userService: IUserService) =
    inherit ControllerBase()

    [<HttpGet>]
    [<Route("users")>]
    member this.List() =
        userService.List() |> Async.StartAsTask

    [<HttpPost>]
    [<Route("users")>]
    member this.Post([<FromBodyAttribute>] userRequest: UserController.UserCreationRequest) =
        
        // Basic validity check.
        // Add better checks.
        match userRequest with
            | { Email = null } | { Email = "" } | { EmailVerify = null } | { EmailVerify = "" }
                -> raise (InputValdationError "Missing email.")
            | { Password = null } | { Password = "" } | { PasswordVerify = null } | { PasswordVerify = "" }
                -> raise (InputValdationError "Missing password.")
            | { Name = null } | { Name = "" }
                -> raise (InputValdationError "Missing name.")
            | _ when userRequest.Email <> userRequest.EmailVerify
                -> raise (InputValdationError "Email does not match.")
            | _ when userRequest.Password <> userRequest.PasswordVerify
                -> raise (InputValdationError "Passwords do not match.")
            | _
                -> ()

        let user: User = {
            Id = ""
            Name = userRequest.Name
            Email = userRequest.Email
            Roles = []
            Created = DateTime.UtcNow
            Updated = DateTime.UtcNow
        }

        async {
            logger.LogInformation($"Received new user: {user}")
            let! user = userService.Create user
            logger.LogInformation($"User created {user}")

            // todo: setup credential

            return user
        }