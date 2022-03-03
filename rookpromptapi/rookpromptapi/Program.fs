namespace rookpromptapi
#nowarn "20"
open System

open Microsoft.Extensions.Hosting

open rookpromptapi.Config

module Program =

    [<EntryPoint>]
    let main args =
        (WebServer.Create args).Run()
        0
