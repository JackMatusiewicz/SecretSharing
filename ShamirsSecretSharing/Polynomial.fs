namespace ShamirsSecretSharing
open System

type private PolynomialTerm = {
    Power : int
    Coefficient : bigint
}

type Polynomial = {
    Terms : PolynomialTerm list
    Prime : bigint
}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Polynomial =

    let create
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
                let coeff = RandomGenerator.generate bigIntGenerator
                let term = {Power = (int thresh); Coefficient = coeff}
                create (thresh - 1u) (term :: acc)
        let terms = create (minimumSegementsToSolve - 1u) []
        {Terms = terms; Prime = prime}

    let evaluate (xValue : bigint) (polynomial : Polynomial) : bigint =
        polynomial.Terms
        |> List.map (fun term -> BigInteger.Pow( xValue, term.Power) * term.Coefficient)
        |> List.map (FiniteFieldElement.fromBigInt (polynomial.Prime))
        |> List.fold (+) (FiniteFieldElement.fromBigInt (polynomial.Prime) (bigint 0))
        |> (fun x -> x.ToBigInt())
