module rookpromptapitest.TestPromptService

open System
open Xunit

open rookpromptapitest
open rookpromptapi.Services

[<Fact>]
let TestPromptServiceList () =
    let promptService = TestServices.GetService<IPromptService>()
    let prompts = promptService.List()
    Assert.NotNull(prompts)
