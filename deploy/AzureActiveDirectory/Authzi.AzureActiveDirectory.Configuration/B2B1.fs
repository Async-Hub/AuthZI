namespace Authzi.AzureActiveDirectory.Configuration

open Authzi.AzureActiveDirectory.Configuration.Common
open Microsoft.Graph

module B2B1 =
    let configure(graphClient : GraphServiceClient) =
        let api1 = new Application()
        api1.DisplayName <- Clients.webClient1
        let mutable result = graphClient.Applications.Request().AddAsync(api1).Result
        printf "Api is created %s" result.AppId
        
        let webClient1 = new Application()
        webClient1.DisplayName <- Clients.webClient1
        result <- graphClient.Applications.Request().AddAsync(webClient1).Result
        printf "WebClient1 is created %s" result.AppId
