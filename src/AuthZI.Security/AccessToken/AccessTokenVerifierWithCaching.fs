namespace AuthZI.Security.AccessToken

open System
open System.Security.Cryptography
open System.Text
open System.Threading.Tasks
open AuthZI.Security.Caching

module Hashing =
    let tryToOptimize accessToken =
        let accessTokenType = AccessTokenAnalyzer.GetTokenType accessToken
        if accessTokenType = AccessTokenType.Reference then accessToken
        else
            use sha256 = SHA256.Create()
            let hashValue = Encoding.UTF8.GetBytes(accessToken) |> sha256.ComputeHash
            let stringBuilder = new StringBuilder()
            hashValue |> Seq.iter (fun elem -> 
                stringBuilder.Append(elem.ToString("x2"))|> ignore)


            stringBuilder.ToString()

type AccessTokenVerifierWithCaching(accessTokenVerifier: IAccessTokenVerifier,
                                    accessTokenCache: IAccessTokenCache, 
                                    cacheEntryExpirationTime: int) =
    interface IAccessTokenVerifier with
        member _.Verify accessToken = 
            async {
                //if String.IsNullOrWhiteSpace accessToken then raise(ArgumentException(nameof accessToken))
                if String.IsNullOrWhiteSpace accessToken then raise(ArgumentException("accessToken"))
                // TODO: Need deeper investigations, maybe this optimization is unnecessary.
                let key = Hashing.tryToOptimize accessToken

                let mutable cacheValue : obj = null
                if accessTokenCache.Current.TryGetValue(key, &cacheValue) then
                    return cacheValue :?> AccessTokenVerificationResult
                else
                    let! verificationResult = 
                        accessTokenVerifier.Verify(accessToken) |> Async.AwaitTask

                    let cacheEntry = accessTokenCache.Current.CreateEntry(key)
                    cacheEntry.Value <- verificationResult
                    cacheEntry.AbsoluteExpiration <- 
                        Nullable<DateTimeOffset>(DateTimeOffset.Now.AddSeconds(Convert.ToDouble(cacheEntryExpirationTime)))

                    return verificationResult } |> Async.StartAsTask