namespace AuthZI.Security.AccessToken

open System.Collections.Generic
open System.Security.Claims
open System.Threading.Tasks

type IAccessTokenVerifier =
    abstract Verify: accessToken : string -> Task<Result<IEnumerable<Claim>, string>>