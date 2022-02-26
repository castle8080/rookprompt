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
type PromptController(
    logger : ILogger<PromptController>,
    promptService: IPromptService) =
    inherit ControllerBase()

    [<HttpGet>]
    [<Route("prompts")>]
    member this.List() =
        promptService.List()

    [<HttpGet>]
    [<Route("prompts/random")>]
    member this.GetRandom() =
        promptService.SampleOne()