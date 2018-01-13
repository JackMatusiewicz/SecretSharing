namespace SecretSharing

open System.Numerics

type Coordinate = {X : bigint; Y : bigint}
type Prime = bigint

module Math =

    let lcm (a : BigInteger) (b : BigInteger) =
        if a = (bigint 0) && b = (bigint 0) then
            (bigint 0)
        else
            let gcd = BigInteger.GreatestCommonDivisor(a,b)
            (a * b) / gcd

    let rec extendedGcd (a : bigint) (b : bigint) =
        match a,b with
        | (a,b) when b = (bigint 0) -> (bigint 1, bigint 0, a)
        | _ ->
            let quotient = BigInteger.Divide(a,b)
            let rem = BigInteger.Remainder(a,b)
            let (x,y,z) = extendedGcd b rem
            (y, x - quotient * y, z)

    let modularInverse (a : bigint) (modulus : bigint) =
        let (i, _, k) = extendedGcd a modulus
        if k = (bigint 1) then
            if i < (bigint 0) then
                Some <| i + modulus
            else
                Some i
        else None

    ///Calculates the modulus, including when the value is negative.
    let (%%) (a : bigint) (modulus : bigint) =
        let rec calc (a : bigint) =
            match a with
            | _ when a >= (bigint 0) ->
                a % modulus
            | _ ->
                let b = (-a / modulus) + (bigint 1)
                (a + (b * modulus)) % modulus
        calc a