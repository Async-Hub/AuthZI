namespace Authzi.Security

open System.Threading.Tasks

type IAccessTokenProvider =
    abstract member RetrieveTokenAsync : unit -> Task<string>