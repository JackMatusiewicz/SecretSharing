namespace SecretSharing

open System
open System.Collections.Generic

type ICustomReconstructor<'secret, 'coord, 'prime> =
    abstract member ReconstructSecret : 'prime * List<'coord> -> 'secret

//Wraps the SecretReconstructor so you can deal with anything, rather than with bigints.
module CustomReconstructor =

    let getPassword
        (fromBigInt : bigint -> 'a)
        (toCoordinate : 'b -> Coordinate)
        (prime : Prime) (encodedCoords : 'b list) : 'a =

        encodedCoords
        |> List.map toCoordinate
        |> SecretReconstructor.getSecret prime
        |> fromBigInt

    [<CompiledName("Make")>]
    let make (fromBigInt : Func<bigint, 'a>, toCoord : Func<'b, Coordinate>, toPrime : Func<'prime, bigint>) =
        { new ICustomReconstructor<_, _, _> with
                member __.ReconstructSecret (prime, coords) : 'a =
                    let f = Function.fromFunc fromBigInt
                    let g = Function.fromFunc toCoord
                    let fromPrime = Function.fromFunc toPrime

                    coords
                    |> List.ofSeq
                    |> getPassword f g (toPrime prime) }
