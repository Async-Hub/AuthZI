﻿using AuthZI.Security;
using AuthZI.Security.AccessToken;
using AuthZI.Security.Authorization;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthZI.MicrosoftOrleans.Authorization
{
  public abstract class GrainAuthorizationFilterBase(
        IAccessTokenVerifier accessTokenVerifier,
        IAuthorizationExecutor authorizeHandler,
        ILogger logger)
    {
        protected readonly ILogger Logger = logger;

        protected async Task<IEnumerable<Claim>> AuthorizeAsync(IGrainCallContext grainCallContext)
        {
          var accessToken = RequestContext.Get(ConfigurationKeys.AccessTokenKey)?.ToString();

          if (string.IsNullOrWhiteSpace(accessToken))
          {
            throw new InvalidOperationException("AccessToken can not be null or empty.");
          }

          var accessTokenVerificationResult = await accessTokenVerifier.Verify(accessToken);
          var isOk = accessTokenVerificationResult.IsOk;

          // ReSharper disable once InvertIf
          if (isOk)
          {
            IEnumerable<Claim> claims = accessTokenVerificationResult.ResultValue;
            IEnumerable<IAuthorizeData> grainAuthorizeData = null;
            var grainMethodAuthorizeData =
              grainCallContext.InterfaceMethod.GetCustomAttributes<AuthorizeAttribute>();

            if (grainCallContext.InterfaceMethod.ReflectedType != null)
            {
              grainAuthorizeData =
                grainCallContext.InterfaceMethod.ReflectedType.GetCustomAttributes<AuthorizeAttribute>();
            }

            var authorizationSucceeded = await authorizeHandler.AuthorizeAsync(claims,
              grainAuthorizeData, grainMethodAuthorizeData);

            if (!authorizationSucceeded)
            {
              throw new AuthorizationException("Access to the requested grain denied.");
            }

            return claims;
          }

          string error = accessTokenVerificationResult.ErrorValue;
          throw new AuthorizationException("Access token verification failed.",
            new InvalidAccessTokenException(error));
        }

        protected void Log(EventId eventId, string grainTypeName, string interfaceMethodName)
        {
            Logger.LogTrace(eventId, $"{eventId.Name} Type of Grain: {grainTypeName} " +
                                     $"Method Name: {interfaceMethodName} ");
        }
    }
}