namespace rookpromptapi.Models

open System

type Prompt = {
    Id: int
    Prompt: string
    Created: DateTime
    Updated: DateTime
}

