namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Identity.DuendeSoftware.IdentityServer
open Duende.IdentityServer.Services
open IdentityModel.Client
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open System
open System.Net.Http

type Startup() =
    member _.ConfigureServices(services: IServiceCollection) =
        services.AddIdentityServer().AddDeveloperSigningCredential()
                .AddInMemoryApiScopes(IdentityServer4Resources.getApiScopes())
                .AddInMemoryApiResources(IdentityServer4Resources.getApiResources())
                .AddInMemoryIdentityResources(IdentityServer4Resources.getIdentityResources())
                .AddInMemoryClients(IdentityServerClients.getClients()).AddTestUsers(Users.getUsers()) |> ignore

        services.AddTransient<IProfileService, ProfileService>() |> ignore
        services.AddControllersWithViews() |> ignore

    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        app.UseStaticFiles().UseIdentityServer().UseRouting()
           .UseEndpoints(fun endpoints -> endpoints.MapDefaultControllerRoute() |> ignore) |> ignore

module IdentityServerBuilder =
    let build(identityServerUrl: string) =
        Host.CreateDefaultBuilder([||])
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseUrls(identityServerUrl).UseStartup<Startup>() |> ignore)

    let private getDiscoveryDocumentAsync (client: HttpClient) =
        async {
            let! discoveryResponse = client.GetDiscoveryDocumentAsync() |> Async.AwaitTask

            let discoveryDocument = DiscoveryDocument()
            discoveryDocument.IntrospectionEndpoint <- discoveryResponse.IntrospectionEndpoint
            discoveryDocument.Issuer <- discoveryResponse.Issuer
            discoveryDocument.Keys <- discoveryResponse.KeySet.Keys
            discoveryDocument.TokenEndpoint <- discoveryResponse.TokenEndpoint

            return discoveryDocument
        }

    let getDiscoveryDocument identityServerUrl  =
        let httpClient = new HttpClient()
        httpClient.BaseAddress <- Uri identityServerUrl
        httpClient |> getDiscoveryDocumentAsync |> Async.RunSynchronously