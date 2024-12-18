namespace AuthZI.MicrosoftEntra

open AuthZI.Identity.MicrosoftEntra
open Microsoft.Identity.Client

// Username/Password flow to acquire a token silently. For testing purposes only!
// https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth-ropc
module AccessTokenRetriever =
    let getTokenByUserNameAndPasswordForEntraIDTenant (entraIDApp: MicrosoftEntraApp) userName password =
        let appConfig = new PublicClientApplicationOptions()
        
        appConfig.TenantId <- entraIDApp.DirectoryId
        appConfig.ClientId <- entraIDApp.ClientId
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

        let result = app.AcquireTokenByUsernamePassword(entraIDApp.AllowedScopes, 
            userName, password.ToString()).ExecuteAsync().Result

        result.AccessToken

    let getTokenByUserNameAndPasswordForEntraExternalIDTenant (entraExternalIDApp: MicrosoftEntraApp) userName password =
        let appConfig = new PublicClientApplicationOptions()
        
        appConfig.TenantId <- entraExternalIDApp.DirectoryId
        appConfig.ClientId <- entraExternalIDApp.ClientId
        appConfig.Instance <- $"https://{entraExternalIDApp.DirectoryId}.ciamlogin.com/"
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg

        let app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

        let result = app.AcquireTokenByUsernamePassword(entraExternalIDApp.AllowedScopes, 
            userName, password.ToString()).ExecuteAsync().Result

        result.AccessToken