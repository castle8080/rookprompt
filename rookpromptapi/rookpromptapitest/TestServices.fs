namespace rookpromptapitest

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

open rookpromptapi.Config
open Microsoft.Extensions.Configuration

module TestServices =

    let private CreateConfiguration () =
        (new ConfigurationBuilder())
            // There was an error finding the secrets id by default.
            // I had to set it explicitly.
            .AddUserSecrets("484e6654-1f4b-4ef1-bbaf-842c679264e0")
            .AddJsonFile("appsettings.json")
            .Build() :> IConfiguration

    let private Configure (serviceCollection: IServiceCollection) =
        serviceCollection
            .AddLogging()
            .AddSingleton(CreateConfiguration())
            |> CoreServices.Configure

    let private lzServiceProvider = lazy(Configure(new ServiceCollection()).BuildServiceProvider())

    let ServiceProvider () = lzServiceProvider.Force()

    let GetService<'a> () = ServiceProvider().GetService<'a>()