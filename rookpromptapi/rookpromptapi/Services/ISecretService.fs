namespace rookpromptapi.Services

open System
open System.Threading.Tasks

open rookpromptapi.Models

type ISecretService =
    
    abstract member Create : Secret -> Secret Async

    abstract member Update : Secret -> Secret Async

    abstract member Delete : string -> bool Async

    abstract member FindById : string -> Secret option Async

    abstract member List : unit -> Secret list Async
