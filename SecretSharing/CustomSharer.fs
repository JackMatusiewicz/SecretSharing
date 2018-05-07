namespace SecretSharing

open System
open Function

type ICustomSharer<'secret,'coord, 'prime> =
    abstract member GenerateCoordinates : uint32 * uint32 * 'secret -> Shares<'coord, 'prime>

//Wraps the SecretSharer so you can deal with anything, rather than with bigints.
module CustomSharer =

    let generateCoordinates
        (toBigInt : 'secret -> bigint)
        (fromCoord : Coordinate -> 'coord)
        (fromPrime : Prime -> 'prime)
        (minimumSegmentsToSolve : uint32)
        (numberOfCoords : uint32)
        (secret : 'secret) : 'prime * 'coord list
        =
        secret
        |> toBigInt
        |> SecretSharer.generateCoordinates minimumSegmentsToSolve numberOfCoords
        |> Tuple.map (List.map fromCoord)
        |> Tuple.leftMap fromPrime

    [<CompiledName("Make")>]
    let make
        (toBigInt : Func<'secret, bigint>,
         fromCoord : Func<Coordinate, 'coord>,
         fromPrime : Func<Prime, 'prime>)
         =
        { new ICustomSharer<_,_,_> with
                member __.GenerateCoordinates (minimumSegmentsToSolve, numberOfCoords, secret) =
                    let toBigInt = Function.fromFunc toBigInt
                    let fromCoord = Function.fromFunc fromCoord
                    let fromPrime = Function.fromFunc fromPrime

                    generateCoordinates
                        toBigInt
                        fromCoord
                        fromPrime
                        minimumSegmentsToSolve
                        numberOfCoords
                        secret
                    |> Tuple.map toGenericList
                    |> Shares.make }
