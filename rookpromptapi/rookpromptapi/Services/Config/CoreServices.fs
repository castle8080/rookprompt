namespace rookpromptapi.Services.Config

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

open rookpromptapi.Services
open rookpromptapi.Services.Mongo
open rookpromptapi.Services.ServiceCollectionExtensions

open MongoDB.Bson
open MongoDB.Driver

/// <summary>
/// Configuration for the core service in rook prompt.
///
/// Configurations in this module should be the same regardless of environment.
///
/// </summary>
module CoreServices =

    let MongoDBClientFactory (sp: IServiceProvider) =
        let config = sp.GetService<IConfiguration>()

        // Replace user name and password in the connection string using secrets.
        let db_connection =
            config["db_connection"]
                .Replace("{db_username}", config["db_username"])
                .Replace("{db_password}", config["db_password"])

        new MongoClient(db_connection)

    let PromptServiceFactory (sp: IServiceProvider) =
        let config = sp.GetService<IConfiguration>()
        new MongoPromptService(
            sp.GetService<MongoClient>(),
            config["db_database"]
        ) :> IPromptService

    let Configure (serviceCollection: IServiceCollection) =
        serviceCollection
            .AddSingleton(MongoDBClientFactory)
            .AddSingleton(PromptServiceFactory)