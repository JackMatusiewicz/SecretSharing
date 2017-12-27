namespace SecretSharing

open Reader
open System

type Share = int * float

//TODO - in order to actually be useful, this must use finite field arithmetic!

type ShareGenerator =
    abstract member GenerateSecret : uint32*uint32*int -> Share list

module SecretSharing =

    type private PolynomialTerm = {
        Power : int
        Coefficient : int
    }

    type private SecretGraph = {
        PolynomialTerms : PolynomialTerm list
    }

    let private createSecretGraph (threshold : uint32) (secret : int) : SecretGraph =
        let rec create (thresh : uint32) (acc : PolynomialTerm list) =
            match thresh with
            | 0u ->
                let constant = {Power = 0; Coefficient = secret}
                (constant :: acc) |> List.rev
            | _ ->
                let term = {Power = (int thresh); Coefficient = 5} //TODO - replace this with random generation
                create (thresh - 1u) (term :: acc)
        let terms = create (threshold - 1u) []
        {PolynomialTerms = terms}

    let private calculateShare (shareNumber : int) (graph : SecretGraph) : Share =
        let floatShare = (float shareNumber)
        graph.PolynomialTerms
        |> List.map (fun term -> Math.Pow(floatShare, (float term.Power)) * (float term.Coefficient))
        |> List.fold (+) 0.0
        |> (fun share -> (shareNumber, share))

    let private createDesiredShares (numberOfShares : int) (graph : SecretGraph) =
        let r = System.Random() //TODO - replace with real source of randomness.
        let rec create (remaining : int) (acc : Share list) =
            match remaining with
            | _ when remaining <= 0 ->
                acc
            | _ ->
                let nextShare = r.Next(5000) //TODO - replace with BigNum so we can have arbitrarily large numbers
                let next = calculateShare nextShare graph
                create (remaining - 1) (next :: acc)
        create numberOfShares []

    let makeGenerator () =
        { new ShareGenerator with
                member __.GenerateSecret (thresh, shares, secret) =
                    createSecretGraph thresh secret
                    |> createDesiredShares (int shares) }

    let private computeBasisPolynomial (vals : Share list) ((thisX,_) : Share) : int -> float =
        vals
        |> List.map fst
        |> List.filter (fun x -> x <> thisX)
        |> List.map (fun x -> fun z -> (float (z - x)) / (float (thisX - x)))
        |> List.fold (fun f g -> (*) <!> f <*> g) (fun _ -> 1.0)

    let private constructPolynomial (vals : Share list) : int -> float =
        vals
        |> List.map (computeBasisPolynomial vals)
        |> List.zip vals
        |> List.map (fun ((_,y), f) -> fun x -> y * (f x))
        |> List.fold (fun f g -> (+) <!> f <*> g) (fun _ -> 0.0)

    let getSecret (threshold : uint32) (shares : Share list) : int =
        if (shares |> List.length |> uint32) < threshold then
            failwithf "Need more than %d shares to compute secret" threshold
        else
            constructPolynomial shares 0 |> Math.Round |> int