namespace SecretSharing

open Reader
open System
open System.Numerics
open Math

type Coordinate = bigint * bigint

type Prime = bigint

type CoordinateGenerator =
    abstract member GenerateCoordinates : uint32*uint32*bigint -> Prime * Coordinate list

type SecretReconstructor =
    abstract member ReconstructSecret : bigint*Coordinate list -> bigint

module SecretSharing =

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

    let makeGenerator () =
        { new CoordinateGenerator with
                member __.GenerateCoordinates (minimumSegementsToSolve, numberOfCoords, secret) =
                    let prime = BigInt.findLargerMersennePrime secret
                    let generator = RandomGenerator.makeRandomBigIntRange prime
                    let poly = Polynomial.create minimumSegementsToSolve generator secret prime
                    poly
                    |> createCoordinates (int numberOfCoords) generator
                    |> (fun shares -> poly.Prime,shares) }

    //TODO - move everything below this into a SecretReconstruction module.
    let private computeBasisPolynomial
        (prime : bigint)
        (vals : Coordinate list)
        ((thisX,_) : Coordinate) : bigint -> FiniteFieldElement =
        vals
        |> List.map fst
        |> List.filter (fun x -> x <> thisX)
        |> List.map (fun xj -> fun x -> BigRational.fromFraction (x - xj) (thisX - xj))
        |> List.map (Reader.map (FiniteFieldElement.fromRational prime))
        |> List.fold (fun f g -> f <?> (*) <*> g) (fun _ -> FiniteFieldElement.fromBigInt prime (bigint 1))

    let private reconstructPolynomial (prime : bigint) (vals : Coordinate list) : bigint -> bigint =
        vals
        |> List.map (computeBasisPolynomial prime vals)
        |> List.zip vals
        |> List.map (fun ((_,y), f) -> fun x -> (f x) * y)
        |> List.map (Reader.map FiniteFieldElement.toBigInt)
        |> List.fold (fun f g -> f <?> (+) <*> g) (fun _ -> (bigint 0))
        |> (Reader.map (fun x -> x %% prime))

    let getSecret (prime : bigint) (shares : Coordinate list) : bigint =
        let f = reconstructPolynomial prime shares
        f (bigint 0)

    let makeReconstructor () =
        { new SecretReconstructor with
                member __.ReconstructSecret (prime, coords) : bigint =
                    getSecret prime coords }