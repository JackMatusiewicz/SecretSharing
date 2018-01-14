namespace SecretSharing

open System.Numerics

type PolynomialTerm = {
    Power : int
    Coefficient : bigint
}

type Polynomial = {
    Terms : PolynomialTerm list
    Prime : bigint
}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Polynomial =

    ///Generates a polynomial with the required number of terms,
    ///The degrees used for the terms range from (0, numberOfTerms-1). Each degree is used once.
    ///The constant parameter allows you to define a particular constant for the polynomial.
    ///Everything else is randomly generated.
    let create
        (numberOfTerms : uint32)
        (bigIntGenerator : RandomGenerator<bigint>)
        (constant : bigint)
        (prime : bigint) : Polynomial =

        let rec create (thresh : uint32) (acc : PolynomialTerm list) =
            match thresh with
            | 0u ->
                let constant = {Power = 0; Coefficient = constant}
                (constant :: acc) |> List.rev
            | _ ->
                let coeff = RandomGenerator.generate bigIntGenerator
                let term = {Power = (int thresh); Coefficient = coeff}
                create (thresh - 1u) (term :: acc)
        let terms = create (numberOfTerms - 1u) []
        {Terms = terms; Prime = prime}

    ///Evaluates a polynomial at a particular x-value.
    let evaluate (x : bigint) (polynomial : Polynomial) : bigint =
        polynomial.Terms
        |> List.map (fun term -> BigInteger.Pow(x, term.Power) * term.Coefficient)
        |> List.map (FiniteFieldElement.fromBigInt (polynomial.Prime))
        |> List.fold (+) (FiniteFieldElement.fromBigInt (polynomial.Prime) (bigint 0))
        |> (FiniteFieldElement.toBigInt)
