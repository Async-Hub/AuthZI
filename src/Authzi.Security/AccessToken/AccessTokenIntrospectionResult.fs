namespace Authzi.Security.AccessToken

open System.Collections.Generic
open System.Security.Claims

type AccessTokenIntrospectionResult(accessTokenType: AccessTokenType,
                                    claims: IEnumerable<Claim>,
                                    isValid: bool, message: string) =
    member _.AccessTokenType = accessTokenType
    member _.Claims = claims
    member _.IsValid = isValid
    member _.Message = message