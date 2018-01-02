namespace SecretSharing

open Reader
open System
open System.Numerics
open Math

type Coordinate = bigint * bigint

type Prime = bigint

type ShareGenerator =
    abstract member GenerateSecret : uint32*uint32*bigint -> Prime * Coordinate list

module SecretSharing =

    type private PolynomialTerm = {
        Power : int
        Coefficient : bigint
    }

    type private SecretGraph = {
        PolynomialTerms : PolynomialTerm list
        Prime : bigint
    }

    let private createSecretGraph
        (threshold : uint32)
        (bigIntGenerator : RandomGenerator<bigint>)
        (secret : bigint)
        (prime : bigint) : SecretGraph =
        let rec create (thresh : uint32) (acc : PolynomialTerm list) =
            match thresh with
            | 0u ->
                let constant = {Power = 0; Coefficient = secret}
                (constant :: acc) |> List.rev
            | _ ->
                let coeff = RandomGeneration.generate bigIntGenerator
                let term = {Power = (int thresh); Coefficient = coeff}
                create (thresh - 1u) (term :: acc)
        let terms = create (threshold - 1u) []
        {PolynomialTerms = terms; Prime = prime}

    let private calculateShare (xValue : bigint) (graph : SecretGraph) : Coordinate =
        graph.PolynomialTerms
        |> List.map (fun term -> BigInteger.Pow( xValue, term.Power) * term.Coefficient)
        |> List.map (FiniteFieldElement.fromBigInt (graph.Prime))
        |> List.fold (+) (FiniteFieldElement.fromBigInt (graph.Prime) (bigint 0))
        |> (fun x -> x.ToBigInt())
        |> (fun share -> (xValue, share))

    let private createDesiredShares (numberOfShares : int) rg (graph : SecretGraph) =
        let rec create (remaining : int) (acc : Coordinate list) =
            match remaining with
            | _ when remaining <= 0 ->
                acc
            | _ ->
                let nextShare = RandomGeneration.generate rg
                let next = calculateShare nextShare graph
                create (remaining - 1) (next :: acc)
        create numberOfShares []
        |> (fun shares -> graph.Prime,shares)

    let makeGenerator () =
        { new ShareGenerator with
                member __.GenerateSecret (thresh, shares, secret) =
                    let prime = BigInt.findLargerMersennePrime secret
                    let generator = RandomGeneration.makeRandomBigIntRange prime
                    createSecretGraph thresh generator secret prime
                    |> createDesiredShares (int shares) generator }

    let private computeBasisPolynomial
        (prime : bigint)
        (vals : Coordinate list)
        ((thisX,_) : Coordinate) : bigint -> FiniteFieldElement =
        vals
        |> List.map fst
        |> List.filter (fun x -> x <> thisX)
        |> List.map (fun xj -> fun x -> BigRational.fromFraction (x - xj) (thisX - xj))
        |> List.map (Reader.map (FiniteFieldElement.fromRational prime))
        |> List.fold (fun f g -> (*) <!> f <*> g) (fun _ -> FiniteFieldElement.fromBigInt prime (bigint 1))

    let private constructPolynomial (prime : bigint) (vals : Coordinate list) : bigint -> bigint =
        vals
        |> List.map (computeBasisPolynomial prime vals)
        |> List.zip vals
        |> List.map (fun ((_,y), f) -> fun x -> (f x) * y)
        |> List.map (Reader.map <| fun x -> x.ToBigInt())
        |> List.fold (fun f g -> (+) <!> f <*> g) (fun _ -> (bigint 0))
        |> (Reader.map (fun x -> x %% prime))

    let getSecret (prime : bigint) (threshold : uint32) (shares : Coordinate list) : bigint =
        if (shares |> List.length |> uint32) < threshold then
            failwithf "Need more than %d shares to compute secret" threshold
        else
            constructPolynomial prime shares (bigint 0)