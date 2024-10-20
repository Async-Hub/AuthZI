namespace AuthZI.Security

open System.Threading.Tasks

type IAccessTokenProvider =
  abstract member RetrieveTokenAsync: unit -> Task<string>
