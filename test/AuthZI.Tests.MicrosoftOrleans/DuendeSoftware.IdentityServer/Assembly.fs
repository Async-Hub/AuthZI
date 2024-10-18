module Assembly

open Xunit

[<assembly: CollectionBehavior(DisableTestParallelization = true)>]
do()

[<assembly: Orleans.ApplicationPartAttribute("Orleans.Persistence.Memory")>]
do()