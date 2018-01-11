namespace SecretSharing

open Reader
open System
open System.Numerics
open Math

type ISecretReconstructor =
    abstract member ReconstructSecret : Prime * Coordinate list -> bigint

module SecretReconstructor =

    let private computeBasisPolynomial
        (prime : bigint)
        (vals : Coordinate list)
        ((xj,_) : Coordinate) : bigint -> FiniteFieldElement =

        vals
        |> List.map fst
        |> List.filter (fun x -> x <> xj)
        |> List.map (fun xm -> fun x -> BigRational.fromFraction (x - xm) (xj - xm))
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

    let make () =
        { new ISecretReconstructor with
                member __.ReconstructSecret (prime, coords) : bigint =
                    getSecret prime coords }
