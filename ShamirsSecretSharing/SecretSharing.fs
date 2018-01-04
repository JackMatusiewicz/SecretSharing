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

    type private PolynomialTerm = {
        Power : int
        Coefficient : bigint
    }

    type private Polynomial = {
        Terms : PolynomialTerm list
        Prime : bigint
    }

    let private createPolynomial
        (minimumSegementsToSolve : uint32)
        (bigIntGenerator : RandomGenerator<bigint>)
        (secret : bigint)
        (prime : bigint) : Polynomial =
        let rec create (thresh : uint32) (acc : PolynomialTerm list) =
            match thresh with
            | 0u ->
                let constant = {Power = 0; Coefficient = secret}
                (constant :: acc) |> List.rev
            | _ ->
                let coeff = RandomGeneration.generate bigIntGenerator
                let term = {Power = (int thresh); Coefficient = coeff}
                create (thresh - 1u) (term :: acc)
        let terms = create (minimumSegementsToSolve - 1u) []
        {Terms = terms; Prime = prime}

    let private getCoordinate (xValue : bigint) (polynomial : Polynomial) : Coordinate =
        polynomial.Terms
        |> List.map (fun term -> BigInteger.Pow( xValue, term.Power) * term.Coefficient)
        |> List.map (FiniteFieldElement.fromBigInt (polynomial.Prime))
        |> List.fold (+) (FiniteFieldElement.fromBigInt (polynomial.Prime) (bigint 0))
        |> (fun x -> x.ToBigInt())
        |> (fun share -> (xValue, share))

    let private createCoordinates
        (numberOfCoordinates : int)
        (rg : RandomGenerator<bigint>)
        (polynomial : Polynomial) =
        let rec create (remaining : int) (acc : Coordinate list) =
            match remaining with
            | _ when remaining <= 0 ->
                acc
            | _ ->
                let x = RandomGeneration.generate rg
                let coordinate = getCoordinate x polynomial
                create (remaining - 1) (coordinate :: acc)
        create numberOfCoordinates []
        |> (fun shares -> polynomial.Prime,shares)

    let makeGenerator () =
        { new CoordinateGenerator with
                member __.GenerateCoordinates (minimumSegementsToSolve, numberOfCoords, secret) =
                    let prime = BigInt.findLargerMersennePrime secret
                    let generator = RandomGeneration.makeRandomBigIntRange prime
                    createPolynomial minimumSegementsToSolve generator secret prime
                    |> createCoordinates (int numberOfCoords) generator }

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

    let private constructPolynomial (prime : bigint) (vals : Coordinate list) : bigint -> bigint =
        vals
        |> List.map (computeBasisPolynomial prime vals)
        |> List.zip vals
        |> List.map (fun ((_,y), f) -> fun x -> (f x) * y)
        |> List.map (Reader.map FiniteFieldElement.toBigInt)
        |> List.fold (fun f g -> f <?> (+) <*> g) (fun _ -> (bigint 0))
        |> (Reader.map (fun x -> x %% prime))

    let getSecret (prime : bigint) (shares : Coordinate list) : bigint =
        let f = constructPolynomial prime shares
        f (bigint 0)

    let makeReconstructor () =
        { new SecretReconstructor with
                member __.ReconstructSecret (prime, coords) : bigint =
                    getSecret prime coords }