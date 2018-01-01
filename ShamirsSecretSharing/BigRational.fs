namespace SecretSharing

open System.Numerics

type BigRational = {
    Numerator : bigint
    Denominator : bigint
}

module BigRational =

    let fromFraction (numerator : bigint) (denomination : bigint) =
        {Numerator = numerator; Denominator = denomination}

    let fromBigInt (numerator : bigint) =
        {Numerator = numerator; Denominator = (bigint 1)}

    let scalarMultiply (c : bigint) (a : BigRational) =
        {Numerator = a.Numerator * c; Denominator = a.Denominator}

    let add (a : BigRational) (b : BigRational) =
        let denomLcm = Math.lcm (a.Denominator) (b.Denominator)
        let t1 = scalarMultiply (denomLcm / b.Denominator) a
        let t2 = scalarMultiply (denomLcm / a.Denominator) b
        {Numerator = t1.Numerator + t2.Numerator; Denominator = denomLcm}

    let sub (a : BigRational) (b : BigRational) =
        let denomLcm = Math.lcm (a.Denominator) (b.Denominator)
        let t1 = scalarMultiply (denomLcm / b.Denominator) a
        let t2 = scalarMultiply (denomLcm / a.Denominator) b
        {Numerator = t1.Numerator - t2.Numerator; Denominator = denomLcm}

    let multiply (a : BigRational) (b : BigRational) =
            {Numerator = a.Numerator * b.Numerator; Denominator = a.Denominator * b.Denominator}

    let divide (n : BigRational) (d : BigRational) =
        {Numerator = n.Numerator * d.Denominator; Denominator = n.Denominator * d.Numerator}
