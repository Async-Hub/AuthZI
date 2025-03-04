﻿using System.Collections.Generic;
using System.Security.Claims;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace AuthZI.Tests.Duende.IdentityServer.Configuration;

public static class TestUsers
{
  public static readonly List<TestUser> Users =
  [
    new TestUser
    {
      SubjectId = "1",
      Username = "alice",
      Password = "Pass123$",

      Claims =
      [
        new Claim(JwtClaimTypes.Name, "Alice Smith"),
        new Claim(JwtClaimTypes.GivenName, "Alice"),
        new Claim(JwtClaimTypes.FamilyName, "Smith"),
        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
        new Claim(JwtClaimTypes.Role, "Admin"),
        new Claim(JwtClaimTypes.Role, "Manager")
      ]
    },
    new TestUser
    {
      SubjectId = "2",
      Username = "bob",
      Password = "Pass123$",

      Claims =
      [
        new Claim(JwtClaimTypes.Name, "Bob Smith"),
        new Claim(JwtClaimTypes.GivenName, "Bob"),
        new Claim(JwtClaimTypes.FamilyName, "Smith"),
        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
        new Claim(JwtClaimTypes.Role, "Developer")
      ]
    }
  ];
}