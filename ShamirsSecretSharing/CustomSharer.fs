namespace SecretSharing

open System
open System.Numerics

type ICustomSharer<'a> =
    abstract member GenerateCoordinates : uint32 * uint32 * 'a -> Prime * Coordinate list

//Wraps the SecretSharer so you can deal with anything, rather than with bigints.
module CustomSharer =

    let generateCoordinates
        (toBigInt : 'a -> bigint)
        (minimumSegmentsToSolve : uint32)
        (numberOfCoords : uint32)
        (secret : 'a) =

        secret
        |> toBigInt
        |> SecretSharer.generateCoordinates minimumSegmentsToSolve numberOfCoords

    let make (convert : Func<'a, bigint>) =
        { new ICustomSharer with
                member __.GenerateCoordinates (minimumSegmentsToSolve, numberOfCoords, secret) =
                    let f = Function.fromFunc convert
                    generateCoordinates f minimumSegmentsToSolve numberOfCoords secret }