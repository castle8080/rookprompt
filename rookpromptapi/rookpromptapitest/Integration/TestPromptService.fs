module rookpromptapitest.Integration.TestPromptService

open System
open System.Threading.Tasks
open Xunit

open rookpromptapitest
open rookpromptapi.Services
open rookpromptapi.Models

[<Fact>]
let TestPromptServiceList () =
    async {
        let promptService = TestServices.GetService<IPromptService>()
        let! prompts = promptService.List()
        Assert.NotNull(prompts)
    } |> Async.StartAsTask

[<Fact>]
let TestPromptServiceSave () =
    async {
        let promptService = TestServices.GetService<IPromptService>()

        let p = Prompt.Create("Describe a moment of fear.")

        // Delete the existing prompt.
        match! promptService.FindByPrompt p.Prompt with
            | None -> ()
            | Some(existing) ->
                promptService.DeleteById(existing.Id) |> ignore

        let! p = promptService.Save(p)
        Assert.True(p.Id.Length > 0)

        let! foundP = promptService.FindByPrompt(p.Prompt)

        Assert.Equal(p.Prompt, foundP.Value.Prompt)
    } |> Async.StartAsTask