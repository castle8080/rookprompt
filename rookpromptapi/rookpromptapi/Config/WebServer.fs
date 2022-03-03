namespace rookpromptapi.Config
#nowarn "20"

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

open rookpromptapi.Middleware

module WebServer =

    let ConfigureServices (builder: WebApplicationBuilder) =
        builder.Services.AddControllers()
        builder.Services |> CoreServices.Configure
        builder

    let ConfigureMiddleWare (app: WebApplication) =
        app.UseMiddleware<ExceptionResponseMapper>()
        app

    let ConfigureApplication (app: WebApplication) =
        app.UseHttpsRedirection()
        app |> ConfigureMiddleWare
        app.UseAuthorization()
        app.MapControllers()
        app.UseStaticFiles()
        app

    let Configure (builder: WebApplicationBuilder) =
        builder |> ConfigureServices
        builder.Build() |> ConfigureApplication

    let Create (args: string[]) =
        WebApplication.CreateBuilder(args) |> Configure
