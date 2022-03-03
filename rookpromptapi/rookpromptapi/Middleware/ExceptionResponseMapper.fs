module rookpromptapi.Middleware

open System
open System.Text
open System.Threading.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

open FSharp.Json

module ExceptionResponseMapper =
    type ExceptionResponse = {
        ErrorType: string
        Message: string
        StackTrace: string
    }

    let GetStatusCode (e: exn): int =
        // TODO: map exceptions to response codes
        500

    let GetResponseBody (e: exn): string =
        let response = {
            ErrorType = e.GetType().Name
            Message = e.Message
            StackTrace = e.StackTrace
        }
        Json.serialize response

type ExceptionResponseMapper(
    reqDelegate: RequestDelegate,
    logger: ILogger<ExceptionResponseMapper>) =

    let HandleException (context: HttpContext) (e: exn): Task =
        task {
            logger.LogInformation $"error: {e}"
            context.Response.StatusCode <- ExceptionResponseMapper.GetStatusCode e
            
            context.Response.ContentType <- "application/json"

            let body = Encoding.UTF8.GetBytes (ExceptionResponseMapper.GetResponseBody e)
            let! r = (context.Response.BodyWriter.WriteAsync body).AsTask()

            return ()
        }

    member this.InvokeAsync (context: HttpContext): Task =
        task {
            try
                do! reqDelegate.Invoke context
            with
                | e -> do! HandleException context e
        } :> Task
