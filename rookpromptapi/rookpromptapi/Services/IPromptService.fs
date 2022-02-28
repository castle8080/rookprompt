namespace rookpromptapi.Services

open System
open System.Threading.Tasks

open rookpromptapi.Models

/// <summary>
/// Service for getting, searching, and creating prompts.
/// </summary>
type IPromptService =
    abstract member Save : Prompt -> Prompt Async

    abstract member List : unit -> Prompt list Async

    abstract member DeleteById : string -> bool Async

    abstract member FindById : string -> Prompt option Async
    
    abstract member FindByPrompt : string -> Prompt option Async

    abstract member SampleOne : unit -> Prompt option Async