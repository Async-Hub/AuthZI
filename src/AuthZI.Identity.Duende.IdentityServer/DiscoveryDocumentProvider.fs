namespace AuthZI.Identity.Duende.IdentityServer

open AuthZI.Security
open IdentityModel.Client
open System
open System.Net.Http

type DiscoveryDocumentProvider
    (clientFactory: IHttpClientFactory, discoveryEndpointUrl: string) =
    let httpClient = clientFactory.CreateClient("IdS")
    let mutable discoveryDocument: DiscoveryDocument = null

    member _.GetDiscoveryDocumentAsync() =
        async {
            if not (isNull discoveryDocument) then
                return discoveryDocument
            else
                let request = new DiscoveryDocumentRequest()
                request.Address <- discoveryEndpointUrl

                let! discoveryResponse = httpClient.GetDiscoveryDocumentAsync(request) |> Async.AwaitTask

                if discoveryResponse.IsError then
                    raise (Exception(discoveryResponse.Error))

                let discoveryDocument = DiscoveryDocument()
                discoveryDocument.IntrospectionEndpoint <- discoveryResponse.IntrospectionEndpoint
                discoveryDocument.Issuer <- discoveryResponse.Issuer
                discoveryDocument.Keys <- discoveryResponse.KeySet.Keys

                return discoveryDocument
        }
        |> Async.StartAsTask