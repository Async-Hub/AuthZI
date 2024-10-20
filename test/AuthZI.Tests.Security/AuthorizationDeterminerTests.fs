namespace AuthZI.Tests.Security

open AuthZI.Security
open AuthZI.Security.Authorization
open Xunit

[<Authorize>]
type TypeThatRequiresAuthorization() =
  member _.MethodWithoutAuthorizeAttribute() = ()

  [<AllowAnonymous>]
  member _.MethodWithAllowAnonymousAttribute() = ()

type TypeThatRequiresAuthorizationOnlyForMethod() =
  [<AllowAnonymous>]
  member _.MethodWithAllowAnonymousAttribute() = ()

  [<Authorize>]
  member _.MethodWithAuthorizeAttribute() = ()

  member _.MethodWithoutAuthorizeAttribute() = ()

type AuthorizationDeterminerTests() =

  [<Fact>]
  member _.``Should return false when AllowAnonymous attribute is present``() =
    // Arrange
    let typeUnderTest = TypeThatRequiresAuthorizationOnlyForMethod()

    let methodInfo =
      typeof<TypeThatRequiresAuthorizationOnlyForMethod>
        .GetMethod(nameof (typeUnderTest.MethodWithAllowAnonymousAttribute))

    // Act
    let result = AuthorizationDeterminer.IsRequired(methodInfo)

    // Assert
    Assert.False(result)

  [<Fact>]
  member _.``Should return false when AllowAnonymous attribute is present on a type that requires authorization``() =
    let testClass = TypeThatRequiresAuthorization()
    // Arrange
    let methodInfo =
      typeof<TypeThatRequiresAuthorization>
        .GetMethod(nameof (testClass.MethodWithAllowAnonymousAttribute))

    // Act
    let result = AuthorizationDeterminer.IsRequired(methodInfo)

    // Assert
    Assert.False(result)
    
  [<Fact>]
  member _.``Should return true when Authorize attribute is present on the type``() =
    let testClass = TypeThatRequiresAuthorization()
    // Arrange
    let methodInfo =
      typeof<TypeThatRequiresAuthorization>
        .GetMethod(nameof (testClass.MethodWithoutAuthorizeAttribute))

    // Act
    let result = AuthorizationDeterminer.IsRequired(methodInfo)

    // Assert
    Assert.True(result)

  [<Fact>]
  member _.``Should return true when Authorize attribute is present on the method``() =
    // Arrange
    let typeUnderTest = TypeThatRequiresAuthorizationOnlyForMethod()

    let methodInfo =
      typeof<TypeThatRequiresAuthorizationOnlyForMethod>
        .GetMethod(nameof (typeUnderTest.MethodWithAuthorizeAttribute))

    // Act
    let result = AuthorizationDeterminer.IsRequired(methodInfo)

    // Assert
    Assert.True(result)

  [<Fact>]
  member _.``Should return false when Authorize attribute are absent``() =
    // Arrange
    let typeUnderTest = TypeThatRequiresAuthorizationOnlyForMethod()

    let methodInfo =
      typeof<TypeThatRequiresAuthorizationOnlyForMethod>
        .GetMethod(nameof (typeUnderTest.MethodWithoutAuthorizeAttribute))

    // Act
    let result = AuthorizationDeterminer.IsRequired(methodInfo)

    // Assert
    Assert.False(result)