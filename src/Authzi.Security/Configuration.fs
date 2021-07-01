namespace Authzi.Security

open System
open Authzi.Security.AccessToken
open Authzi.Security.Authorization

type Configuration()=
    member val ConfigureAccessTokenVerifierOptions : Action<AccessTokenVerifierOptions> = null with get,set
    member val ConfigureAuthorizationOptions : Action<AuthorizationOptions> = null with get,set
    member val ConfigureSecurityOptions: Action<SecurityOptions> = null with get,set
    member val TracingEnabled = false with get, set