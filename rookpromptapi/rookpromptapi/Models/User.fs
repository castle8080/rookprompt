namespace rookpromptapi.Models

open System

type User = {
    Id: string
    Name: string
    Email: string
    Roles: string list
    Created: DateTime
    Updated: DateTime
}