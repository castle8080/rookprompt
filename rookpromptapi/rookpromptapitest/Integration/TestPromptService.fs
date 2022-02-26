module rookpromptapitest.Integration.TestPromptService

open System
open Xunit

open rookpromptapitest
open rookpromptapi.Services
open rookpromptapi.Models

[<Fact>]
let TestPromptServiceList () =
    let promptService = TestServices.GetService<IPromptService>()
    let prompts = promptService.List()
    Assert.NotNull(prompts)

[<Fact>]
let TestPromptServiceSave () =
    let promptService = TestServices.GetService<IPromptService>()

    let p = Prompt.Create("Describe a moment of fear.")

    // Delete the existing prompt.
    match promptService.FindByPrompt p.Prompt with
        | None -> ()
        | Some(existing) ->
            promptService.DeleteById(existing.Id) |> ignore

    let p = promptService.Save(p)

    Assert.True(p.Id.Length > 0)
