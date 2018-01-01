namespace SecretSharing

open Reader
open System
open System.Numerics
open Math

type Share = int * bigint

//TODO - in order to actually be useful, this must use finite field arithmetic!

type ShareGenerator =
    abstract member GenerateSecret : uint32*uint32*bigint -> Share list

module SecretSharing =

    type private PolynomialTerm = {
        Power : int
        Coefficient : bigint
    }

    type private SecretGraph = {
        PolynomialTerms : PolynomialTerm list
    }

    let private createSecretGraph
        (threshold : uint32)
        (bigIntGenerator : RandomGeneration<bigint>)
        (secret : bigint) : SecretGraph =
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
        {PolynomialTerms = terms}

    let private calculateShare (shareNumber : int) (graph : SecretGraph) : Share =
        let xValue = (bigint shareNumber)
        graph.PolynomialTerms
        |> List.map (fun term -> BigInteger.Pow(xValue, term.Power) * term.Coefficient)
        |> List.fold (+) (bigint 0)
        |> (fun bi -> bi %% (bigint 65537))
        |> (fun share -> (shareNumber, share))

    let private createDesiredShares (numberOfShares : int) (graph : SecretGraph) =
        let rec create (remaining : int) (acc : Share list) =
            match remaining with
            | _ when remaining <= 0 ->
                acc
            | _ ->
                let nextShare = remaining //TODO - replace with random element in finite field.
                let next = calculateShare nextShare graph
                create (remaining - 1) (next :: acc)
        create numberOfShares []

    let makeGenerator () =
        { new ShareGenerator with
                member __.GenerateSecret (thresh, shares, secret) =
                    createSecretGraph thresh (RandomGeneration.makeRandomBigintGenerator 4) secret
                    |> createDesiredShares (int shares) }

    let private biMult (a : BigRational) (b : BigRational) : BigRational = //TODO - move pls
        (a * b) % (bigint 65537)

    let private computeBasisPolynomial (vals : Share list) ((thisX,_) : Share) : int -> BigRational =
        vals
        |> List.map fst
        |> List.filter (fun x -> x <> thisX)
        |> List.map (fun xj -> fun x -> BigRational.fromFraction (bigint (x - xj)) (bigint (thisX - xj)))
        |> List.fold (fun f g -> biMult <!> f <*> g) (fun _ -> BigRational.fromBigInt (bigint 1))

    let private toBigInt (modulus : bigint) (br : BigRational) = //TODO - move to FFA
        let modInvDen = Math.modularInverse (br.Denominator) modulus
        match modInvDen with
        | None -> failwithf "%s modinv %s is not valid" ((br.Denominator).ToString()) (modulus.ToString())
        | Some mid -> (br.Numerator) * mid

    let private constructPolynomial (vals : Share list) : int -> bigint =
        vals
        |> List.map (computeBasisPolynomial vals)
        |> List.zip vals
        |> List.map (fun ((_,y), f) -> fun x -> (f x) * y)
        |> List.map (fun f -> toBigInt (bigint 65537) <!> f)
        |> List.fold (fun f g -> (+) <!> f <*> g) (fun _ -> (bigint 0))
        |> (fun f -> fun x -> (f x) %% (bigint 65537))

    let getSecret (threshold : uint32) (shares : Share list) : bigint =
        if (shares |> List.length |> uint32) < threshold then
            failwithf "Need more than %d shares to compute secret" threshold
        else
            constructPolynomial shares 0
