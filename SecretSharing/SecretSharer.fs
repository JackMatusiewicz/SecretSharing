namespace SecretSharing

open Reader
open System
open System.Collections.Generic
open System.Numerics
open Math
open Function

type ISecretSharer =
    abstract member GenerateCoordinates : uint32 * uint32 * bigint -> Prime * List<Coordinate>

module SecretSharer =

    let private createCoordinates
        (numberOfCoordinates : int)
        (rg : RandomGenerator<bigint>)
        (polynomial : Polynomial) =

        let rec create (remaining : int) (acc : Coordinate list) =
            match remaining with
            | _ when remaining <= 0 ->
                acc
            | _ ->
                let x = RandomGenerator.generate rg
                let y = Polynomial.evaluate x polynomial
                create (remaining - 1) ((x,y) :: acc)
        create numberOfCoordinates []

    let generateCoordinates
        (minimumSegmentsToSolve : uint32)
        (numberOfCoords : uint32)
        (secret : bigint) =

        let prime = BigInt.findLargerMersennePrime secret
        let generator = RandomGenerator.makeRandomBigIntRange prime

        let poly = Polynomial.create minimumSegmentsToSolve generator secret prime
        poly
        |> createCoordinates (int numberOfCoords) generator
        |> Tuple.make poly.Prime

    let make () =
        { new ISecretSharer with
                member __.GenerateCoordinates (minimumSegmentsToSolve, numberOfCoords, secret) =
                    generateCoordinates minimumSegmentsToSolve numberOfCoords secret
                    |> Tuple.map toGenericList }
