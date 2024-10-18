namespace AuthZI.Security.AccessToken

open System.Threading.Tasks

type public IAccessTokenIntrospectionService =
    abstract IntrospectTokenAsync: accessToken:string
     -> allowOfflineValidation:bool -> Task<AccessTokenIntrospectionResult>