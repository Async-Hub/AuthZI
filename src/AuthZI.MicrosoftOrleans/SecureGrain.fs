namespace AuthZI.MicrosoftOrleans.Authorization

open AuthZI.Extensions.TaskExtensions
open AuthZI.Security
open AuthZI.Security.AccessToken
open AuthZI.Security.Authorization
open Microsoft.Extensions.Logging
open Orleans
open Orleans.Runtime
open System
open System.Collections.Generic
open System.Reflection
open System.Security.Claims
open System.Threading.Tasks

type SecureGrainContext(accessTokenVerifier: IAccessTokenVerifier, accessTokenProvider: IAccessTokenProvider,
    authorizeHandler: IAuthorizationExecutor, claimTypeResolver: IClaimTypeResolver,
    orleansAuthorizationConfiguration: OrleansAuthorizationConfiguration,
    logger: ILogger<SecureGrainContext>)=
  member _.AccessTokenVerifier = accessTokenVerifier
  member _.AccessTokenProvider = accessTokenProvider
  member _.AuthorizeHandler = authorizeHandler
  member _.ClaimTypeResolver = claimTypeResolver
  member _.OrleansAuthorizationConfiguration = orleansAuthorizationConfiguration
  member _.Logger = logger

type public SecureGrain(secureGrainContext: SecureGrainContext) as this =
  inherit Grain()

  member val User : ClaimsPrincipal = null with get, set

  member private this.RetriveAccessToken(grainCallContext: IIncomingGrainCallContext) =
    task {
      let isCoHostingEnabled = secureGrainContext.OrleansAuthorizationConfiguration.IsCoHostingEnabled
      let mutable accessToken = RequestContext.Get(ConfigurationKeys.AccessTokenKey)

      if (accessToken = null || String.IsNullOrWhiteSpace(accessToken.ToString())) && not isCoHostingEnabled then
        return Error("AccessToken cannot be null or empty.")
      elif not isCoHostingEnabled then
        return Ok(accessToken.ToString())
      else
        let! newAccessToken = secureGrainContext.AccessTokenProvider.RetrieveTokenAsync()
        if String.IsNullOrWhiteSpace(newAccessToken) then
          return Error("AccessToken cannot be null or empty.")
        else
          return Ok(newAccessToken)

      //RequestContext.Set(ConfigurationKeys.AccessTokenKey, accessToken)
    }

  member private this.AuthorizeAsync(grainCallContext: IIncomingGrainCallContext)=
    task {
        let! accessTokenResult = this.RetriveAccessToken(grainCallContext)
        
        match accessTokenResult with
        | Error error -> return Error error
        | Ok accessToken ->
        let! accessTokenVerificationResult = secureGrainContext.AccessTokenVerifier.Verify(accessToken)

        match accessTokenVerificationResult with
        | Ok claims ->
            let grainMethodAuthorizeData: IEnumerable<IAuthorizeData> = 
                grainCallContext.InterfaceMethod.GetCustomAttributes<AuthorizeAttribute>()
                |> Seq.cast<IAuthorizeData>

            let mutable grainAuthorizeData: IEnumerable<IAuthorizeData> = null

            if grainCallContext.InterfaceMethod.ReflectedType <> null then
                grainAuthorizeData <- grainCallContext.InterfaceMethod.ReflectedType.GetCustomAttributes<AuthorizeAttribute>()
                                      |> Seq.cast<IAuthorizeData>

            let! authorizationSucceeded = 
              secureGrainContext.AuthorizeHandler.AuthorizeAsync(claims, grainAuthorizeData, grainMethodAuthorizeData)
            if authorizationSucceeded then
                return Ok claims
            else
                return Error("Authorization failed.")
        | Error error -> return Error error
    }


  interface IIncomingGrainCallFilter with
    member _.Invoke(context: IIncomingGrainCallContext) =
      task {
        if AuthorizationDeterminer.IsRequired context.InterfaceMethod then
          let grainType = context.Grain.GetType()
          let! authorizeResult = this.AuthorizeAsync(context)

          match authorizeResult with
          | Ok claims ->
              let subject = secureGrainContext.ClaimTypeResolver.Resolve(ClaimType.Subject)
              let role = secureGrainContext.ClaimTypeResolver.Resolve(ClaimType.Role)
              let claimsIdentity = new ClaimsIdentity(claims, "", subject, role)
              this.User <- new ClaimsPrincipal(claimsIdentity)

              secureGrainContext.Logger.LogDebug(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                  grainType.Name, context.InterfaceMethod.Name)
          | Error error ->
              secureGrainContext.Logger.LogInformation(LoggingEvents.IncomingGrainCallAuthorizationFailed,
                  grainType.Name, context.InterfaceMethod.Name, error)
              raise (AuthorizationException("Access to the requested grain denied."))
                 
          secureGrainContext.Logger.LogDebug(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                        grainType.Name, context.InterfaceMethod.Name)

        do! context.Invoke()
      }
  
  interface IOutgoingGrainCallFilter with
    member _.Invoke(context: IOutgoingGrainCallContext) =
      task {
        if AuthorizationDeterminer.IsRequired(context.InterfaceMethod) then
          let mutable accessToken : String = null
          accessToken <- (RequestContext.Get(ConfigurationKeys.AccessTokenKey) |? (String.Empty :> Object)).ToString()

          if String.IsNullOrWhiteSpace(accessToken) then
            let! accessToken = secureGrainContext.AccessTokenProvider.RetrieveTokenAsync()

            if String.IsNullOrWhiteSpace(accessToken) then
              raise (InvalidOperationException("AccessToken can not be null or empty."))

            RequestContext.Set(ConfigurationKeys.AccessTokenKey, accessToken)

        do! context.Invoke()
      }