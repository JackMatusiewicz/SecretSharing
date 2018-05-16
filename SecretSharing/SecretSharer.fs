namespace SecretSharing

open Function
open System

type ISecretSharer =
    abstract member GenerateCoordinates : ThresholdSchemeData * bigint -> Shares<Coordinate, Prime>

module SecretSharer =

    let private createCoordinates
        (numberOfCoordinates : int)
        (rg : RandomGenerator<bigint>)
        (polynomial : Polynomial)
        =
        let rec create (remaining : int) (acc : Coordinate list) =
            match remaining with
            | _ when remaining <= 0 ->
                acc
            | _ ->
                let x = RandomGenerator.generate rg
                let y = Polynomial.evaluate x polynomial
                create (remaining - 1) ({X=x; Y=y} :: acc)
        create numberOfCoordinates []

    let generateCoordinates
        (ts : ThresholdSchemeData)
        (secret : bigint)
        : Prime * Coordinate list
        =
        let prime = BigInt.findLargerMersennePrime secret
        let generator = RandomGenerator.makeRandomBigIntRange prime

        let poly = Polynomial.create ts.NumberOfSharesForRecovery generator secret prime
        poly
        |> createCoordinates (int ts.NumberOfSharesToMake) generator
        |> Tuple.make poly.Prime

    [<CompiledName("Make")>]
    let make () =
        { new ISecretSharer with
                member __.GenerateCoordinates (ts, secret) =
                    generateCoordinates ts secret
                    |> Tuple.map toGenericList
                    |> Shares.make
        }