namespace rookpromptapi.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open rookpromptapi
open rookpromptapi.Models

[<ApiController>]
[<Route("[controller]")>]
type PromptController (logger : ILogger<PromptController>) =
    inherit ControllerBase()

    let summaries =
        [|
            "Freezing"
            "Bracing"
            "Chilly"
            "Cool"
            "Mild"
            "Warm"
            "Balmy"
            "Hot"
            "Sweltering"
            "Scorching"
        |]

    [<HttpGet>]
    member this.Get() =
        let rng = System.Random()
        [|
            for index in 0..4 ->
                { Date = DateTime.Now.AddDays(float index)
                  TemperatureC = rng.Next(-20,55)
                  Summary = summaries.[rng.Next(summaries.Length)] }
        |]
