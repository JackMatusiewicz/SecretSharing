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
        (toPrime : 'prime -> Prime)
        (prime : 'prime) (encodedCoords : 'b list) : 'a =

        encodedCoords
        |> List.map toCoordinate
        |> SecretReconstructor.getSecret (toPrime prime)
        |> fromBigInt

    [<CompiledName("Make")>]
    let make (fromBigInt : Func<bigint, 'a>, toCoord : Func<'b, Coordinate>, toPrime : Func<'prime, Prime>) =
        { new ICustomReconstructor<_, _, _> with
                member __.ReconstructSecret (prime, coords) : 'a =
                    let f = Function.fromFunc fromBigInt
                    let g = Function.fromFunc toCoord
                    let toPrime = Function.fromFunc toPrime

                    coords
                    |> List.ofSeq
                    |> getPassword f g toPrime prime }