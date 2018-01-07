namespace SecretSharing

open System
open System.Numerics

type ICustomReconstructor<'a, 'b> =
    abstract member ReconstructSecret : Prime * 'b list -> 'a

//Wraps the SecretReconstructor so you can deal with anything, rather than with bigints.
module CustomReconstructor =

    let getPassword
        (fromBigInt : bigint -> 'a)
        (toCoordinate : 'b -> Coordinate)
        (prime : bigint) (encodedCoords : 'b list) : 'a =

        encodedCoords
        |> List.map toCoordinate
        |> SecretReconstructor.getSecret prime
        |> fromBigInt

    let make (fromBigInt : Func<bigint, 'a>) (toCoord : Func<'b, Coordinate>) =
        { new ICustomReconstructor<_, _> with
                member __.ReconstructSecret (prime, coords) : 'a =
                    let f = Function.fromFunc fromBigInt
                    let g = Function.fromFunc toCoord
                    getPassword f g prime coords }