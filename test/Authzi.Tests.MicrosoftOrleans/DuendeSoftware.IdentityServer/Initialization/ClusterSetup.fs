module ClusterSetup

open Orleans
open Authzi.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
open System.Threading.Tasks

let initDocumentsRegistry (getClusterClient: string -> IClusterClient) =
    let getToken =
        async {
            let! accessTokenResponse = AccessTokenFactory.getAccessTokenForUserOnWebClient1Async "Alice" "Pass123$"
                                           "Api1 Orleans" |> Async.AwaitTask
            return accessTokenResponse.AccessToken }

    let accessToken = getToken |> Async.RunSynchronously
    let client = getClusterClient accessToken
    
    let document1 = Document()
    document1.Author <- Users.aliceId
    document1.Content <- "Some content 1."
    document1.Name <- "Document1"
    
    let document2 = Document()
    document2.Author <- Users.bobId
    document2.Content <- "Some content 2."
    document2.Name <- "Document2"
    
    let grain = client.GetGrain<IDocumentsRegistryGrain>(DocumentsRegistry.Default)
    grain.Add(document1).Wait()
    grain.Add(document2).Wait()