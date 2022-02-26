namespace rookpromptapi.Services

open System
open rookpromptapi.Models

/// <summary>
/// Service for getting, searching, and creating prompts.
/// </summary>
type IPromptService =
    abstract member Save : Prompt -> Prompt

    abstract member List : unit -> Prompt list