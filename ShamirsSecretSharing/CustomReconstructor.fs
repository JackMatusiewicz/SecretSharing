namespace SecretSharing

open System
open System.Numerics

type ICustomReconstructor<'a, 'b> =
    abstract member ReconstructSecret : Prime * 'b list -> 'a

//Wraps the SecretReconstructor so you can deal with anything, rather than with bigints.
module CustomReconstructor =

    let getPassword<'a>
        (fromBigInt : bigint -> 'a)
        (toCoordinate : 'b -> Coordinate)
        (prime : bigint) (encodedCoords : 'b list) =

        encodedCoords
        |> List.map toCoordinate
        SecretReconstructor.getSecret prime
        |> convert

    let make (fromBigInt : Func<bigint, 'a>) (toCoord : Func<'b, Coordinate>) =
        { new ICustomReconstructor<_, _> with
                member __.ReconstructSecret (prime, coords) : string =
                    let f = Function.fromFunc fromBigInt
                    let g = Function.fromFunc toCoord
                    getPassword<_> f g prime coords }