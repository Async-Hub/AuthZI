namespace Authzi.Security.FSharp

open System.Reflection
open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("Authzi.Security.Client")>]
[<assembly: InternalsVisibleTo("Authzi.Security.Clustering")>]
[<assembly: InternalsVisibleTo("Authzi.Security.Interoperability")>]
[<assembly: InternalsVisibleTo("Authzi.Security.Tests")>]
//TODO: Check why we need this.
[<assembly: AssemblyVersion("1.3.0.0")>]
[<assembly: AssemblyFileVersion("1.3.0.0")>]
[<assembly: AssemblyInformationalVersion("1.3.0.0")>]
do()