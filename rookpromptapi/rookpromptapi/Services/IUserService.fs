namespace rookpromptapi.Services

open System
open System.Threading.Tasks

open rookpromptapi.Models

type IUserService =
    
    abstract member Update : User -> User SysResult Async

    abstract member Create : User -> User SysResult Async

    abstract member FindByEmail : string -> User option Async

    abstract member FindById : string -> User option Async

