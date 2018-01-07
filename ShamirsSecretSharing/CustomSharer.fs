namespace SecretSharing

open System
open System.Numerics

type ICustomSharer<'a,'b> =
    abstract member GenerateCoordinates : uint32 * uint32 * 'a -> Prime * 'b list

//Wraps the SecretSharer so you can deal with anything, rather than with bigints.
module CustomSharer =

    let generateCoordinates<'a, 'b>
        (toBigInt : 'a -> bigint)
        (fromCoord : Coordinate -> 'b)
        (minimumSegmentsToSolve : uint32)
        (numberOfCoords : uint32)
        (secret : 'a) =

        secret
        |> toBigInt
        |> SecretSharer.generateCoordinates minimumSegmentsToSolve numberOfCoords
        |> fun (prime, coords) -> (p, List.map fromCoord coords)

    let make (toBigInt : Func<'a, bigint>) (fromCoord : Func<Coord, 'b>) =
        { new ICustomSharer<_> with
                member __.GenerateCoordinates (minimumSegmentsToSolve, numberOfCoords, secret) =
                    let f = Function.fromFunc toBigInt
                    let g = Function.fromFunc fromCoord
                    generateCoordinates f g minimumSegmentsToSolve numberOfCoords secret }