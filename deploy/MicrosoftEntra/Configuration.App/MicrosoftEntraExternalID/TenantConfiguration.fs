namespace AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalID

open AuthZI.Deploy.MicrosoftEntra.Configuration.AccessTokenRetrieval.AccessTokenProvider
open AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalIDSecrets
open AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftGraph
open AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalID.Api

module TenantConfiguration = 
    let start =
        async {
            let! accessToken = getAccessTokenByUsernamePassword(accessTokenConfig)
            printfn "Access Token for Azure REST API: \n%s" accessToken

            let! tenantId = getTenantIdByName accessToken newTenantName
            printfn "Tenant Name: [ %s ] Tenant Id: [ %s ]" newTenantName tenantId

            let newAccessTokenConfig = {
                accessTokenConfig with
                    TenantId = tenantId
                    Scopes = [
                        "https://graph.microsoft.com/Application.ReadWrite.All";
                        "https://graph.microsoft.com/User.ReadWrite.All"
                    ]
            }

            (* 
              To create a new service principal in the newly created tenant,
              please visit the following URL (replace teanatId and clientId ) 
              and grant the necessary permissions. 
            *)
            // https://login.microsoftonline.com/{tenantId}/adminconsent?client_id={clientId}

            let! accessToken = getAccessTokenByUsernamePassword(newAccessTokenConfig)
            printfn "Access Token for Graph Client: \n%s" accessToken

            let graphClient = getGraphServiceClient accessToken
            do! Applications.configure graphClient newTenantName
        }
