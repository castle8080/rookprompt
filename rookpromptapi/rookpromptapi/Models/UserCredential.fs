namespace rookpromptapi.Models

open System

type UserCredential = {
    Id: string
    Secret: string
    Created: DateTime
    Updated: DateTime
}