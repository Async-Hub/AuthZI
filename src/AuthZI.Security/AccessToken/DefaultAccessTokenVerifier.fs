namespace AuthZI.Security.AccessToken

open System

type DefaultAccessTokenVerifier(options: AccessTokenVerifierOptions,
                                         introspectionService: IAccessTokenIntrospectionService) =
    interface IAccessTokenVerifier with
        member _.Verify accessToken =
            async {
                if String.IsNullOrWhiteSpace accessToken then
                    raise (ArgumentException("The value of accessToken can't be null or empty."))

                let! introspectionResult = introspectionService.IntrospectTokenAsync accessToken
                                               options.AllowOfflineValidation |> Async.AwaitTask
                
                return introspectionResult
            }
            |> Async.StartAsTask