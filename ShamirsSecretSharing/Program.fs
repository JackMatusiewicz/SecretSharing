open System
open System.Numerics

//TODO - in order to actually be useful, this must use finite field arithmetic!

type Secret = {
    Value : bigint
    Threshold : uint32
}

type PolynomialTerm = {
    Power : int
    Coefficient : bigint
}

type SecretGraph = { //TODO - make private
    PolynomialTerms : PolynomialTerm list
}

type Share = {
    ShareNumber : int //x
    Share : bigint //y
}

let createSecretGraph (threshold : uint32) (secret : bigint) : SecretGraph =
    let rec create (thresh : uint32) (acc : PolynomialTerm list) =
        match thresh with
        | 0u ->
            let constant = {Power = 0; Coefficient = secret}
            (constant :: acc) |> List.rev
        | _ ->
            let term = {Power = (int thresh); Coefficient = (bigint 5)} //TODO - replace this with random generation
            create (thresh - 1u) (term :: acc)
    let terms = create (threshold - 1u) []
    {PolynomialTerms = terms}

let calculateShare (shareNumber : int) (graph : SecretGraph) : Share =
    let bigIntShare = (bigint shareNumber)
    graph.PolynomialTerms
    |> List.map (fun term -> BigInteger.Pow(bigIntShare, term.Power) * term.Coefficient)
    |> List.fold (+) (bigint 0)
    |> (fun share -> {ShareNumber = shareNumber; Share = share})

//This is the Share Computation side of SSS, once this is done, the graph can be destroyed.
let createDesiredShares (numberOfShares : int) (graph : SecretGraph) =
    let r = System.Random() //TODO - replace with real source of randomness.
    let rec create (remaining : int) (acc : Share list) =
        match remaining with
        | _ when remaining <= 0 -> 
            acc
        | _ ->
            let nextShare = r.Next()
            let next = calculateShare nextShare graph
            create (remaining - 1) (next :: acc)
    create numberOfShares []  

//This is the secret reconstruction part of SSS. Using the threshold of the secret and the number of shares we have,
//we can reconstruct the graph.
let constructGraph (shares : Share list) (threshold : uint32) =
    if (shares |> List.length |> uint32) < threshold then
        failwithf "Need more than %d shares to compute secret" threshold
    else
        failwith "TODO"           

[<EntryPoint>]
let main argv = 
    let mySecret = (bigint 1303)
    let graph = createSecretGraph 2u mySecret
    let share = createDesiredShares 3 graph
    printfn "%A" share
    printfn "%A" graph
    0 // return an integer exit code
