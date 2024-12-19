namespace AuthZI.Security.Authorization

open System.Collections.Generic
open System.Security.Claims
open System.Threading.Tasks
open CSharpFunctionalExtensions

type IAuthorizationExecutor =
    abstract member AuthorizeAsync : 
        claims: IEnumerable<Claim> * 
        typeAuthorizeData: IEnumerable<IAuthorizeData> * 
        methodAuthorizeData: IEnumerable<IAuthorizeData> -> Task<Result<ClaimsPrincipal, string>>
