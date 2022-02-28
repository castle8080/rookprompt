namespace rookpromptapi.Services

open System
open System.Threading.Tasks

open rookpromptapi.Models

type IUserCredentialService =
    
    abstract member Save : UserCredential -> UserCredential Task

    abstract member GetByUserId : string -> UserCredential Task