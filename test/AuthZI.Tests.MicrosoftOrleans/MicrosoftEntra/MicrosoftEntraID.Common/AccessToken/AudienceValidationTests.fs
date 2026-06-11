namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open AccessTokenProvider
open AuthZI.Security.AccessToken
open AuthZI.Tests.Common.Xunit
open AuthZI.Identity.MicrosoftEntra
open System.IdentityModel.Tokens.Jwt
open Xunit

type AudienceValidationTestsBase(output: ITestOutputHelper) =

  let getAudienceClaimValues (accessToken: string) =
    let token = JwtSecurityTokenHandler().ReadJwtToken(accessToken)
    token.Claims |> Seq.filter (fun claim -> claim.Type = "aud") |> Seq.map (fun claim -> claim.Value) |> Seq.toArray

  [<Theory>]
  [<MemberData(nameof (TestData.Users), MemberType = typeof<TestData>, DisableDiscoveryEnumeration = true)>]
  member _.``Real access token audience is included in expected audiences for configured app`` (userName: string) (password: string) =
    task {
      let! accessToken = getAccessTokenForUserOnWebClient1Async userName password
      output.WriteLine(accessToken)

      let audienceClaims = getAudienceClaimValues accessToken
      let expectedAudiences = TestData.Web1ClientApp.ValidAudiences |> Seq.toArray

      let hasExpectedAudience =
        audienceClaims |> Seq.exists (fun aud -> expectedAudiences |> Array.contains aud)

      Assert.True(hasExpectedAudience)
    }

  [<Theory>]
  [<MemberData(nameof (TestData.Users), MemberType = typeof<TestData>, DisableDiscoveryEnumeration = true)>]
  member _.``Real access token from another app has audience outside configured expected audiences`` (userName: string) (password: string) =
    task {
      let! accessToken = getAccessTokenForUserOnWebClient2Async userName password
      output.WriteLine(accessToken)

      let audienceClaims = getAudienceClaimValues accessToken
      let expectedAudiences = TestData.Web1ClientApp.ValidAudiences |> Seq.toArray

      let hasUnexpectedAudience =
        audienceClaims |> Seq.exists (fun aud -> expectedAudiences |> Array.contains aud |> not)

      Assert.True(hasUnexpectedAudience)
    }
