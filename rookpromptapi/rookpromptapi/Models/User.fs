namespace rookpromptapi.Models

open System

type User = {
    Id: int
    Name: string
    Email: string
    Created: DateTime
    Updated: DateTime
}