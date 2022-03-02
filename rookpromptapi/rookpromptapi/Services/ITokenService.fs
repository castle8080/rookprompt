namespace rookpromptapi.Services

open System
open System.Text

open rookpromptapi.Models

type ITokenService =
    abstract member Create : User -> string Async

    abstract member Validate : string -> User Async