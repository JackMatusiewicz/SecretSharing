namespace SecretSharing

open System
open Function

type ICustomSharer<'secret,'coord, 'prime> =
    abstract member GenerateCoordinates : uint32 * uint32 * 'secret -> Shares<'coord, 'prime>

//Wraps the SecretSharer so you can deal with anything, rather than with bigints.
module CustomSharer =

    let generateCoordinates
        (toBigInt : 'a -> bigint)
        (fromCoord : Coordinate -> 'b)
        (fromPrime : Prime -> 'prime)
        (minimumSegmentsToSolve : uint32)
        (numberOfCoords : uint32)
        (secret : 'a) : 'prime * 'b list =

        secret
        |> toBigInt
        |> SecretSharer.generateCoordinates minimumSegmentsToSolve numberOfCoords
        |> Tuple.map (List.map fromCoord)
        |> Tuple.leftMap fromPrime

    [<CompiledName("Make")>]
    let make (toBigInt : Func<'a, bigint>, fromCoord : Func<Coordinate, 'b>, fromPrime : Func<Prime, 'prime>) =
        { new ICustomSharer<_,_,_> with
                member __.GenerateCoordinates (minimumSegmentsToSolve, numberOfCoords, secret) =
                    let f = Function.fromFunc toBigInt
                    let g = Function.fromFunc fromCoord
                    let fromPrime = Function.fromFunc fromPrime

                    generateCoordinates f g fromPrime minimumSegmentsToSolve numberOfCoords secret
                    |> Tuple.map toGenericList
                    |> Shares.make }
