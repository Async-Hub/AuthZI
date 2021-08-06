namespace Authzi.AzureActiveDirectory

open Authzi.Security
open System
open System.Net.Http

type DiscoveryDocumentProvider(clientFactory: IHttpClientFactory, discoveryEndpointUrl: string) =
    let httpClient = clientFactory.CreateClient("ActiveDirectoryClientConfig")
    let mutable discoveryDocument: DiscoveryDocument = null

    member _.GetDiscoveryDocumentAsync() =
        async {
            if not (isNull discoveryDocument) then
                return discoveryDocument
            else
                
                let! discoveryResponse = httpClient.GetAsync(discoveryEndpointUrl) |> Async.AwaitTask
                
                if not discoveryResponse.IsSuccessStatusCode then raise (Exception(""))

                let discoveryDocument = DiscoveryDocument()

                return discoveryDocument
        } |> Async.StartAsTask