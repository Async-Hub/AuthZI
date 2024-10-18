module IdentityServerResources

open Duende.IdentityServer.Models
open IdentityModel
open System.Collections.Generic

let api1Value = "Api1"
let orleansValue = "Orleans"

let getApiScopes() =
    let api1Scope = new ApiScope(api1Value, api1Value)
    let orleansScope = new ApiScope(orleansValue, orleansValue)

    [api1Scope; orleansScope] |> ResizeArray<ApiScope>

let getApiResources() =
    let api1 = ApiResource api1Value
    api1.UserClaims.Add JwtClaimTypes.Email
    api1.UserClaims.Add JwtClaimTypes.Role
    api1.Scopes.Add api1Value

    Secret(HashExtensions.Sha256 "Secret") |> api1.ApiSecrets.Add
    
    let orleans = ApiResource(orleansValue);
    Secret(HashExtensions.Sha256 "@3x3g*RLez$TNU!_7!QW") |> orleans.ApiSecrets.Add
    orleans.Scopes.Add orleansValue

    [api1; orleans] |> ResizeArray<ApiResource>

let getIdentityResources() =
    let resources = List<IdentityResource>()

    resources.Add(IdentityResources.Email())
    resources.Add(IdentityResources.Profile())
    resources.Add(IdentityResources.OpenId())

    resources