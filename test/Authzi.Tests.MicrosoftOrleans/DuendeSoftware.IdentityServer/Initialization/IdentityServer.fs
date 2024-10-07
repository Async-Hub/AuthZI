module IdentityServer

open Authzi.MicrosoftOrleans.IdentityServer4.Tests
open Duende.IdentityServer.Services
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

type Startup() =
    member _.ConfigureServices(services: IServiceCollection) =
        services.AddIdentityServer().AddDeveloperSigningCredential()
                .AddInMemoryApiScopes(IdentityServer4Resources.getApiScopes())
                .AddInMemoryApiResources(IdentityServer4Resources.getApiResources())
                .AddInMemoryIdentityResources(IdentityServer4Resources.getIdentityResources())
                .AddInMemoryClients(IdSClients.getClients()).AddTestUsers(Users.getUsers()) |> ignore

        services.AddTransient<IProfileService, ProfileService>() |> ignore
        services.AddControllersWithViews() |> ignore

    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        app.UseStaticFiles().UseIdentityServer().UseRouting()
           .UseEndpoints(fun endpoints -> endpoints.MapDefaultControllerRoute() |> ignore) |> ignore

let createHostBuilder =
    Host.CreateDefaultBuilder([||])
        .ConfigureWebHostDefaults(fun webBuilder ->
            webBuilder.UseUrls(GlobalConfig.identityServer4Url).UseStartup<Startup>() |> ignore)

let startServer() =
    createHostBuilder.Build().RunAsync()