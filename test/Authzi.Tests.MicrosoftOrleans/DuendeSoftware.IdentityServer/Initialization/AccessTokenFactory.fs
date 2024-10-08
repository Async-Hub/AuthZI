module AccessTokenFactory

open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open IdentityModel.Client
open System.Net.Http

let getAccessTokenForClientAsync (clientId: string) (clientSecret: string) (scope: string) =
    let tokenRequest = new ClientCredentialsTokenRequest(
        Address = discoveryDocument.TokenEndpoint,
        Scope = scope, ClientId = clientId,
        ClientSecret = clientSecret)

    let httpClient = new HttpClient()
    httpClient.RequestClientCredentialsTokenAsync(tokenRequest)
    
let getAccessTokenForUserAsync (clientId: string) (clientSecret: string) (userName: string)
    (password: string) (scope: string) =
    let tokenRequest = new PasswordTokenRequest(
        Address = discoveryDocument.TokenEndpoint,
        ClientId = clientId,
        ClientSecret = clientSecret,
        UserName = userName,
        Password = password,
        Scope = scope)

    let httpClient = new HttpClient()
    httpClient.RequestPasswordTokenAsync(tokenRequest)

let getAccessTokenForUserOnWebClient1Async = getAccessTokenForUserAsync WebClient1 "Secret1"
let getAccessTokenForUserOnWebClient2Async = getAccessTokenForUserAsync WebClient2 "Secret2"