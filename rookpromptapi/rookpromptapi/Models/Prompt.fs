namespace rookpromptapi.Models

open System

type Prompt = {
    Id: string
    Prompt: string
    Created: DateTime
    Updated: DateTime
}

module Prompt =
    let Create (prompt: string): Prompt = {
        Id = ""
        Prompt = prompt
        Created = DateTime.UtcNow
        Updated = DateTime.UtcNow
    }