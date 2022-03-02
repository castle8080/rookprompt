namespace rookpromptapi.Models

open System

type Token<'a> = {
    Expires: DateTime
    Payload: 'a
}