namespace Authzi.Security.AccessToken

open Microsoft.Extensions.Logging
open System.Diagnostics

type AccessTokenVerifierWithTracing(isCachingEnabled, 
                                    accessTokenVerifier : IAccessTokenVerifier,
                                    logger: ILogger<IAccessTokenVerifier>) =
        interface IAccessTokenVerifier with
            member _.Verify accessToken= 
                async{
                    let stopwatch = new Stopwatch()
                    stopwatch.Start()
                    
                    let! result = accessTokenVerifier.Verify accessToken |> Async.AwaitTask
                    
                    stopwatch.Stop()
                    let message =  System.String.Format(" Time: {0} ms. CachingEnabled: {1}", 
                                    stopwatch.ElapsedMilliseconds, isCachingEnabled)
                    
                    // TODO: Implement logging.
                    //logger.LogInformation(LoggingEvents.AccessTokenVerified, message)
                    
                    return result
                } |> Async.StartAsTask
