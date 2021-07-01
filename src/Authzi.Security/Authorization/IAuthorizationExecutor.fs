namespace Authzi.Security.Authorization

open System.Collections.Generic
open System.Security.Claims
open System.Threading.Tasks

type internal IAuthorizationExecutor =
    abstract member AuthorizeAsync : 
        claims: IEnumerable<Claim> * 
        grainInterfaceAuthorizeData: IEnumerable<IAuthorizeData> * 
        grainMethodAuthorizeData: IEnumerable<IAuthorizeData> -> Task<bool>
