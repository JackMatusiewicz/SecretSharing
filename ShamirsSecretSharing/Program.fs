open System
open System.Numerics

module Reader =

    let map (f : 'a -> 'b) (x : 'r -> 'a) : 'r -> 'b =
        x >> f
    let (<!>) = map

    let apply (f : 'r -> 'a -> 'b) (x : 'r -> 'a) : 'r -> 'b =
        fun r -> f r (x r)
    let (<*>) = apply

open Reader

type Share = int * float

//TODO - in order to actually be useful, this must use finite field arithmetic!

type ShareGenerator =
    abstract member GenerateSecret : uint32*uint32*int -> Share list

module SecretDistribution =

    type Secret = {
        Value : int
        Threshold : uint32
    }

    type PolynomialTerm = {
        Power : int
        Coefficient : int
    }

    type SecretGraph = { //TODO - make private
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

    //This is the Share Computation side of SSS, once this is done, the graph can be destroyed.
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

    let make () =
        { new ShareGenerator with
                member __.GenerateSecret (thresh, shares, secret) =
                    createSecretGraph thresh secret
                    |> createDesiredShares (int shares) }

module SecretReconstruction =

    let private interpolate (vals : Share list) ((thisX,_) : Share) : int -> float =
        vals
        |> List.filter (fun (x,_) -> x <> thisX)
        |> List.map (fun (x,_) -> fun z -> (float (z - x)) / (float (thisX - x)))
        |> List.fold (fun f g -> (*) <!> f <*> g) (fun _ -> 1.0)

    let private constructPolynomial (vals : Share list) : int -> float =
        vals
        |> List.map (interpolate vals)
        |> List.zip vals
        |> List.map (fun ((_,y), f) -> fun x -> y * (f x))
        |> List.fold (fun f g -> (+) <!> f <*> g) (fun _ -> 0.0)

    let getSecret (threshold : uint32) (shares : Share list) : float =
        if (shares |> List.length |> uint32) < threshold then
            failwithf "Need more than %d shares to compute secret" threshold
        else
            constructPolynomial shares 0
            
            
let test () =  
    let mySecret = 11234
    let generator = SecretDistribution.make()
    let shares = generator.GenerateSecret (3u, 6u, mySecret)
    let secret = SecretReconstruction.getSecret 3u shares
    printfn "%f" secret

[<EntryPoint>]
let main argv =
    0
