namespace AuthZI.Security

open System
open AuthZI.Security.AccessToken
open AuthZI.Security.Authorization

type Configuration()=
    member val ConfigureAccessTokenVerifierOptions : Action<AccessTokenVerifierOptions> = null with get,set
    member val ConfigureAuthorizationOptions : Action<AuthorizationOptions> = null with get,set