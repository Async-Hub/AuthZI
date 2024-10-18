namespace AuthZI.Security

open System
open AuthZI.Security.AccessToken
open AuthZI.Security.Authorization

type Configuration()=
    member val ConfigureAccessTokenVerifierOptions : Action<AccessTokenVerifierOptions> = null with get,set
    member val ConfigureAuthorizationOptions : Action<AuthorizationOptions> = null with get,set
    member val ConfigureSecurityOptions: Action<SecurityOptions> = null with get,set
    member val TracingEnabled = false with get, set