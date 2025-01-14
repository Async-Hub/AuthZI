namespace AuthZI.MicrosoftOrleans.Authorization

open AuthZI.Security
open Microsoft.Extensions.Logging
open Orleans
open Orleans.Runtime
open System
open System.Security.Claims
open System.Security

type SecureGrainContext(accessTokenExtractor: AccessTokenExtractor,admissionExecutor: AdmissionExecutor,
  logger: ILogger<SecureGrainContext>)=
  
  member _.AccessTokenExtractor = accessTokenExtractor
  member _.AdmissionExecutor = admissionExecutor
  member _.Logger = logger

type public SecureGrain(secureGrainContext: SecureGrainContext) as this =
  inherit Grain()

  static member AccessDeniedMessage = "Access to the requested grain denied."

  member val User : ClaimsPrincipal = null with get, set

  interface IIncomingGrainCallFilter with
    member _.Invoke(context: IIncomingGrainCallContext) =
      task {
        if AuthorizationDeterminer.IsRequired context.InterfaceMethod then
          let grainType = context.Grain.GetType()

          let accessToken = RequestContext.Get(ConfigurationKeys.AccessTokenKey)
          let! accessTokenResult = secureGrainContext.AccessTokenExtractor.RetriveAccessToken(accessToken)
          let! authorizeResult = secureGrainContext.AdmissionExecutor.AdmitAsync(accessTokenResult, context.InterfaceMethod)

          match authorizeResult with
          | Ok claimsPrincipal ->
              this.User <- claimsPrincipal

              secureGrainContext.Logger.LogDebug(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                  grainType.Name, context.InterfaceMethod.Name)
          | Error error ->
              secureGrainContext.Logger.LogInformation(LoggingEvents.IncomingGrainCallAuthorizationFailed,
                  grainType.Name, context.InterfaceMethod.Name, error)
              raise (SecurityException(SecureGrain.AccessDeniedMessage))
          
          secureGrainContext.Logger.LogDebug(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                        grainType.Name, context.InterfaceMethod.Name)

        do! context.Invoke()
      }
  
  interface IOutgoingGrainCallFilter with
    member _.Invoke(context: IOutgoingGrainCallContext) =
      task {
        if AuthorizationDeterminer.IsRequired(context.InterfaceMethod) then
          let accessToken = RequestContext.Get(ConfigurationKeys.AccessTokenKey)
          let! accessTokenResult = secureGrainContext.AccessTokenExtractor.RetriveAccessToken(accessToken)

          match accessTokenResult with
          | Error error -> raise (InvalidOperationException(error))
          | Ok accessToken -> RequestContext.Set(ConfigurationKeys.AccessTokenKey, accessToken)

        do! context.Invoke()
      }