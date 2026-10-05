module Assembly

open Xunit.v3
open Xunit.Sdk

[<assembly: Parallelization(Mode = ParallelMode.None)>]
do()

[<assembly: Orleans.ApplicationPartAttribute("Orleans.Persistence.Memory")>]
do()