namespace SecretSharing

open System
open System.Numerics

type ICustomReconstructor<'a> =
    abstract member ReconstructSecret : bigint * Coordinate list -> 'a

//Wraps the SecretReconstructor so you can deal with anything, rather than with bigints.
module CustomReconstructor =

    let getPassword<'a>
        (convert : bigint -> 'a)
        (prime : bigint) (coords : Coordinate list) =

        SecretReconstructor.getSecret prime coords
        |> convert

    let make (convert : Func<bigint, 'a>) =
        { new ICustomReconstructor<_> with
                member __.ReconstructSecret (prime, coords) : string =
                    let f = Function.fromFunc convert
                    getPassword f prime coords }