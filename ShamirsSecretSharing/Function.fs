namespace SecretSharing

open System
open System.Collections.Generic
open System.Linq

module Function =

    let fromFunc (f : Func<'a, 'b>) : 'a -> 'b =
        fun a -> f.Invoke(a)
        
    let toGenericList (a : 'a list) : List<'a> =
        a
        |> Array.ofList
        |> fun a -> a.ToList()