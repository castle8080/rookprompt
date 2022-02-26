namespace rookpromptapi.Services

open System
open rookpromptapi.Models

/// <summary>
/// Service for getting, searching, and creating prompts.
/// </summary>
type IPromptService =
    abstract member Save : Prompt -> Prompt

    abstract member List : unit -> Prompt list

    abstract member DeleteById : string -> bool

    abstract member FindById : string -> Prompt option
    
    abstract member FindByPrompt : string -> Prompt option

    abstract member SampleOne : unit -> Prompt option