namespace AuthZI.MicrosoftOrleans.Authorization

open AuthZI.Security
open System

type public AccessTokenExtractor(accessTokenProvider: IAccessTokenProvider, 
  authorizationConfiguration: AuthorizationConfiguration)=
  member _.AccessTokenProvider = accessTokenProvider
  member _.OrleansAuthorizationConfiguration = authorizationConfiguration

  member public this.RetriveAccessToken(accessToken: obj) =
    task {
      let isCoHostingEnabled =  this.OrleansAuthorizationConfiguration.IsCoHostingEnabled

      if (accessToken = null || String.IsNullOrWhiteSpace(accessToken.ToString())) && not isCoHostingEnabled then
        return Error("AccessToken cannot be null or empty.")
      elif not isCoHostingEnabled then
        return Ok(accessToken.ToString())
      else
        let! newAccessToken = this.AccessTokenProvider.RetrieveTokenAsync()
        if String.IsNullOrWhiteSpace(newAccessToken) then
          return Error("AccessToken cannot be null or empty.")
        else
          return Ok(newAccessToken)
    }