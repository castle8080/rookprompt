namespace rookpromptapi.Services.ServiceCollectionExtensions

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

open System.Collections.Generic
open System.Runtime.CompilerServices

[<Extension>]
type ServiceCollectionExtensions =

    /// <summary>
    /// Extends IServiceCollection to make adding a singleton using an F# function eaiser.
    /// </summary>
    [<Extension>]
    static member AddSingleton<'a when 'a : not struct> (this: IServiceCollection, f: IServiceProvider -> 'a) =
        let _f : Func<IServiceProvider, 'a> = f
        this.AddSingleton<'a>(_f) |> ignore
        this