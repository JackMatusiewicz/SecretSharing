namespace SecretSharing

open Function
open System.Collections.Generic

type ISecretSharer =
    abstract member GenerateCoordinates : ThresholdSchemeData * bigint -> Shares<Coordinate, Prime>

module SecretSharer =

    let private createCoordinates
        (numberOfCoordinates : int)
        (rg : RandomGenerator<bigint>)
        (prime : Prime)
        (polynomial : Polynomial)
        =
        let rec create (remaining : int) (acc : Coordinate list) (xsVisited : bigint HashSet) =
            match remaining with
            | _ when remaining <= 0 ->
                acc
            | _ ->
                let x = RandomGenerator.generate rg % prime
                if xsVisited.Contains x then
                    create remaining acc xsVisited
                else
                    xsVisited.Add x |> ignore
                    let y = Polynomial.evaluate x polynomial
                    create (remaining - 1) ({X=x; Y=y} :: acc) xsVisited
        create numberOfCoordinates [] (HashSet<_> ())

    let generateCoordinates
        (ts : ThresholdSchemeData)
        (secret : bigint)
        : Prime * Coordinate list
        =
        let prime = BigInt.findLargerMersennePrime secret
        let generator = RandomGenerator.makeRandomBigIntRange prime

        Polynomial.create ts.NumberOfSharesForRecovery generator secret prime
        |> fun polynomial -> (polynomial.Prime, polynomial)
        |> Tuple.map (createCoordinates (int ts.NumberOfSharesToMake) generator prime)

    [<CompiledName("Make")>]
    let make () =
        { new ISecretSharer with
                member __.GenerateCoordinates (ts, secret) =
                    generateCoordinates ts secret
                    |> Tuple.map toGenericList
                    |> Shares.make
        }