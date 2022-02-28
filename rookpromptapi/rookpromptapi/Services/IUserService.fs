namespace rookpromptapi.Services

open System
open System.Threading.Tasks

open rookpromptapi.Models

type IUserService =
    
    abstract member Update : User -> User Async

    abstract member Create : User -> User Async

    abstract member FindByEmail : string -> User option Async

    abstract member FindById : string -> User option Async

