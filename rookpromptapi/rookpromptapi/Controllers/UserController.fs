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

[<ApiController>]
type UserController(
    logger : ILogger<UserController>,
    userService: IUserService) =
    inherit ControllerBase()

    [<HttpGet>]
    [<Route("users")>]
    member this.List() =
        userService.List() |> Async.StartAsTask
