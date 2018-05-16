namespace SecretSharing

open System
open Function

type ICustomSharer<'secret,'coord, 'prime> =
    abstract member GenerateCoordinates : ThresholdSchemeData * 'secret -> Shares<'coord, 'prime>

//Wraps the SecretSharer so you can deal with anything, rather than with bigints.
module CustomSharer =

    let generateCoordinates
        (toBigInt : 'secret -> bigint)
        (fromCoord : Coordinate -> 'coord)
        (fromPrime : Prime -> 'prime)
        (ts : ThresholdSchemeData)
        (secret : 'secret) : 'prime * 'coord list
        =
        secret
        |> toBigInt
        |> SecretSharer.generateCoordinates ts
        |> Tuple.map (List.map fromCoord)
        |> Tuple.leftMap fromPrime

    [<CompiledName("Make")>]
    let make
        (toBigInt : Func<'secret, bigint>,
         fromCoord : Func<Coordinate, 'coord>,
         fromPrime : Func<Prime, 'prime>)
         =
        { new ICustomSharer<_,_,_> with
                member __.GenerateCoordinates (ts, secret) =
                    let toBigInt = Function.fromFunc toBigInt
                    let fromCoord = Function.fromFunc fromCoord
                    let fromPrime = Function.fromFunc fromPrime

                    generateCoordinates
                        toBigInt
                        fromCoord
                        fromPrime
                        ts
                        secret
                    |> Tuple.map toGenericList
                    |> Shares.make }
