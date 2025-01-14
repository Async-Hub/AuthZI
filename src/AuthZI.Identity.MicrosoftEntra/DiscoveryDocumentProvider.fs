namespace AuthZI.MicrosoftEntra

open Microsoft.IdentityModel.Protocols
open Microsoft.IdentityModel.Protocols.OpenIdConnect

type DiscoveryDocumentProvider(discoveryEndpointUrl: string) =
  let mutable discoveryDocument: DiscoveryDocument option = None
  let discoveryEndpointUrl = discoveryEndpointUrl

  member _.GetDiscoveryDocumentAsync() =
    task {
      if discoveryDocument.IsSome then
        return discoveryDocument
      else
        let configManager =
          ConfigurationManager<OpenIdConnectConfiguration>(discoveryEndpointUrl, OpenIdConnectConfigurationRetriever())

        let! config = configManager.GetConfigurationAsync()
        discoveryDocument <- Some(DiscoveryDocument(discoveryEndpointUrl, config.SigningKeys))

        return discoveryDocument
    }