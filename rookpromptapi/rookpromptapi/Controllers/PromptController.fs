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

    [<HttpPost>]
    [<Route("prompts")>]
    member this.Post([<FromBodyAttribute>] prompt: Prompt) =
        let prompt = {
            prompt with
                Id = prompt.Id |> Option.ofObj |> Option.defaultValue ""
                Updated = DateTime.UtcNow
                Created = DateTime.UtcNow
        }
        
        logger.LogInformation($"Received new prompt: {prompt}")
        
        promptService.Save(prompt)