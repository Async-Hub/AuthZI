namespace AuthZI.Identity.MicrosoftEntra

open CSharpFunctionalExtensions
open Microsoft.Identity.Client
open System
open System.Collections.Generic
open System.Linq

/// <summary>
/// Configuration constants for Microsoft Entra discovery endpoints and URLs.
/// </summary>
[<AbstractClass; Sealed>]
type Configuration =
    static let DiscoveryEndpointPathValue = ".well-known/openid-configuration"
    static let UrlValue = "https://login.microsoftonline.com/{DirectoryId}/v2.0"

    /// <summary>Gets the discovery endpoint path.</summary>
    static member DiscoveryEndpointPath: string = DiscoveryEndpointPathValue

    /// <summary>Gets the base Microsoft Entra URL template.</summary>
    static member Url: string = UrlValue

/// <summary>
/// Base class for Microsoft Entra applications. Provides configuration and discovery.
/// </summary>
[<AbstractClass>]
type MicrosoftEntraApp(
    directoryId: string,
    clientId: string,
    clientSecret: string,
    allowedScopes: IEnumerable<string>,
    aadAuthorityAudience: AadAuthorityAudience) =

    /// <summary>Gets the directory (tenant) ID.</summary>
    member _.DirectoryId: string = directoryId

    /// <summary>Gets the application client ID.</summary>
    member _.ClientId: string = clientId

    /// <summary>Gets the client secret.</summary>
    member _.ClientSecret: string = clientSecret

    /// <summary>Gets the allowed scopes for this application.</summary>
    member _.AllowedScopes: IEnumerable<string> = allowedScopes

    /// <summary>Gets the AAD authority audience type.</summary>
    member _.AadAuthorityAudience: AadAuthorityAudience = aadAuthorityAudience

    ///// <summary>Gets the valid audiences for token validation.</summary>
    //member _.ValidAudiences: IEnumerable<string> = validAudiences

    /// <summary>Gets the token issuer URL for this application.</summary>
    abstract member IssuerUrl: string with get

    /// <summary>Gets the OpenID Connect discovery endpoint URL.</summary>
    abstract member DiscoveryEndpointUrl: string with get

/// <summary>
/// Microsoft Entra ID application configuration (for organizational tenants).
/// </summary>
type MicrosoftEntraIDApp(
    directoryId: string,
    clientId: string,
    clientSecret: string,
    allowedScopes: IEnumerable<string>,
    aadAuthorityAudience: AadAuthorityAudience) =

    inherit MicrosoftEntraApp(
        directoryId,
        clientId,
        clientSecret,
        allowedScopes,
        aadAuthorityAudience)

    /// <summary>Gets the token issuer URL.</summary>
    override _.IssuerUrl: string =
        String.Format("https://login.microsoftonline.com/{0}/v2.0", directoryId)

    /// <summary>Gets the OpenID Connect discovery endpoint URL.</summary>
    override _.DiscoveryEndpointUrl: string =
        String.Format(
            "{0}/{1}",
            Configuration.Url.Replace("{DirectoryId}", directoryId),
            Configuration.DiscoveryEndpointPath)

    /// <summary>Gets an empty application instance for testing purposes.</summary>
    static member EmptyApp: MicrosoftEntraIDApp =
        MicrosoftEntraIDApp("", "", "", [], AadAuthorityAudience.AzureAdMyOrg)

/// <summary>
/// Microsoft Entra External ID application configuration (for customer tenants/CIAM).
/// </summary>
type MicrosoftEntraExternalIDApp(
    directoryId: string,
    clientId: string,
    clientSecret: string,
    allowedScopes: IEnumerable<string>,
    aadAuthorityAudience: AadAuthorityAudience) =

    inherit MicrosoftEntraApp(
        directoryId,
        clientId,
        clientSecret,
        allowedScopes,
        aadAuthorityAudience)

    /// <summary>Gets the token issuer URL.</summary>
    override _.IssuerUrl: string =
        String.Format("https://{0}.ciamlogin.com/{0}/v2.0", directoryId)

    /// <summary>Gets the OpenID Connect discovery endpoint URL.</summary>
    override _.DiscoveryEndpointUrl: string =
        String.Format(
            "https://{0}.ciamlogin.com/{0}/v2.0/.well-known/openid-configuration",
            directoryId)