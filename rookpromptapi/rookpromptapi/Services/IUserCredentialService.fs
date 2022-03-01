namespace rookpromptapi.Services

open System
open System.Threading.Tasks

open rookpromptapi.Models

type IUserCredentialService =
    
    abstract member Save : UserCredential -> UserCredential Async

    abstract member FindByUserId : string -> UserCredential option Async