namespace rookpromptapi.Models

open System

type Secret = {
    Id: string
    Secret: string
    Description: string
    Expires: DateTime
    Created: DateTime
    Updated: DateTime
}