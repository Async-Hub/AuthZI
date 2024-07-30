namespace Authzi.MicrosoftEntra

open Microsoft.IdentityModel.Protocols;  
open Microsoft.IdentityModel.Protocols.OpenIdConnect

type DiscoveryDocumentProvider(discoveryEndpointUrl: string) =
    let mutable discoveryDocument: DiscoveryDocument option = None
    let discoveryEndpointUrl = discoveryEndpointUrl

    member _.GetDiscoveryDocumentAsync() =
        async {
            if discoveryDocument.IsSome then
                return discoveryDocument
            else
               let configManager = ConfigurationManager<OpenIdConnectConfiguration>(discoveryEndpointUrl, 
                                    new OpenIdConnectConfigurationRetriever())

               let! config = configManager.GetConfigurationAsync() |> Async.AwaitTask
               discoveryDocument <- Some(DiscoveryDocument(discoveryEndpointUrl, config.SigningKeys))

               return discoveryDocument
        } |> Async.StartAsTask