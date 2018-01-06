namespace SecretSharing

open System

module Function =

    let fromFunc (f : Func<'a, 'b>) : 'a -> 'b =
        fun a -> f.Invoke(a)
