namespace rookpromptapi.Models

open System

type UserCredential = {
    Id: string
    UserId: string
    Secret: string
    Created: DateTime
    Updated: DateTime
}