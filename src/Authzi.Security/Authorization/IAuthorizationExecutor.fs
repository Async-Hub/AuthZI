namespace Authzi.Security.Authorization

open System.Collections.Generic
open System.Security.Claims
open System.Threading.Tasks

type internal IAuthorizationExecutor =
    abstract member AuthorizeAsync : 
        claims: IEnumerable<Claim> * 
        typeAuthorizeData: IEnumerable<IAuthorizeData> * 
        methodAuthorizeData: IEnumerable<IAuthorizeData> -> Task<bool>
