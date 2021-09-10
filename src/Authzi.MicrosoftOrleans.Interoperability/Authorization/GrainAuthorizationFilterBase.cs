﻿using Authzi.Security;
using Authzi.Security.AccessToken;
using Authzi.Security.Authorization;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authzi.MicrosoftOrleans.Authorization
{
    public abstract class GrainAuthorizationFilterBase
    {
        private readonly IAuthorizationExecutor _authorizeHandler;

        private readonly IAccessTokenVerifier _accessTokenVerifier;

        protected readonly ILogger Logger;

        protected GrainAuthorizationFilterBase(IAccessTokenVerifier accessTokenVerifier, 
            IAuthorizationExecutor authorizeHandler, ILogger logger)
        {
            _authorizeHandler = authorizeHandler;
            _accessTokenVerifier = accessTokenVerifier;
            Logger = logger;
        }

        protected async Task<IEnumerable<Claim>> AuthorizeAsync(IGrainCallContext grainCallContext)
        {
            var accessToken = RequestContext.Get(ConfigurationKeys.AccessTokenKey)?.ToString();
            
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidOperationException("AccessToken can not be null or empty.");
            }

            var accessTokenVerificationResult = await _accessTokenVerifier.Verify(accessToken);

            // ReSharper disable once InvertIf
            if (accessTokenVerificationResult.IsVerified)
            {
                IEnumerable<IAuthorizeData> grainAuthorizeData = null;
                var grainMethodAuthorizeData = grainCallContext.InterfaceMethod.GetCustomAttributes<AuthorizeAttribute>();

                if (grainCallContext.InterfaceMethod.ReflectedType != null)
                {
                    grainAuthorizeData =
                        grainCallContext.InterfaceMethod.ReflectedType.GetCustomAttributes<AuthorizeAttribute>();
                }

                var authorizationSucceeded = await _authorizeHandler.AuthorizeAsync(accessTokenVerificationResult.Claims,
                    grainAuthorizeData, grainMethodAuthorizeData);

                if (!authorizationSucceeded)
                {
                    throw new NotAuthorizedException("Access to the requested grain denied.");
                }

                return accessTokenVerificationResult.Claims;
            }

            throw new NotAuthorizedException("Access token verification failed.",
                new InvalidAccessTokenException(accessTokenVerificationResult.InvalidValidationMessage));
        }

        protected void Log(EventId eventId, string grainTypeName, string interfaceMethodName)
        {
            Logger.LogTrace(eventId, $"{eventId.Name} Type of Grain: {grainTypeName} " +
                                     $"Method Name: {interfaceMethodName} ");
        }
    }
}
