namespace rookpromptapi.Services

open System
open System.Threading.Tasks

open rookpromptapi.Models

/// <summary>
/// Service for getting, searching, and creating prompts.
/// </summary>
type IPromptService =
    abstract member Save : Prompt -> Prompt Task

    abstract member List : unit -> Prompt list Task

    abstract member DeleteById : string -> bool Task

    abstract member FindById : string -> Prompt option Task
    
    abstract member FindByPrompt : string -> Prompt option Task

    abstract member SampleOne : unit -> Prompt option Task