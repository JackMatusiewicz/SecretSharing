namespace SecretSharing

open System.Numerics

type BigRational = {
    Numerator : bigint
    Denominator : bigint
} with
    static member (*) (a : BigRational, b : bigint) =
        {a with Numerator = a.Numerator * b}

    static member (*) (a : BigRational, b : BigRational) =
        {Numerator = a.Numerator * b.Numerator; Denominator = a.Denominator * b.Denominator}

    static member (/) (n : BigRational, d : BigRational) =
        {Numerator = n.Numerator * d.Denominator; Denominator = n.Denominator * d.Numerator}

    static member (+) (a : BigRational, b : BigRational) =
        let denomLcm = Math.lcm (a.Denominator) (b.Denominator)
        let t1 = a * (denomLcm / b.Denominator)
        let t2 = b * (denomLcm / a.Denominator)
        {Numerator = t1.Numerator + t2.Numerator; Denominator = denomLcm}

    static member (-) (a : BigRational, b : BigRational) =
        let denomLcm = Math.lcm (a.Denominator) (b.Denominator)
        let t1 = a * (denomLcm / a.Denominator)
        let t2 = b * (denomLcm / b.Denominator)
        {Numerator = t1.Numerator - t2.Numerator; Denominator = denomLcm}

module BigRational =

    let fromFraction (numerator : bigint) (denomination : bigint) =
        {Numerator = numerator; Denominator = denomination}

    let fromBigInt (numerator : bigint) =
        {Numerator = numerator; Denominator = (bigint 1)}