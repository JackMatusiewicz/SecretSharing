namespace SecretSharing

///An element of a particular finite field. Only has the operations required for secret sharing.
type FiniteFieldElement = {
    Modulus : bigint
    Element : BigRational
} with
    static member AssertSameField (lhs : FiniteFieldElement, rhs : FiniteFieldElement) =
        match lhs.Modulus = rhs.Modulus with
        | true -> ()
        | false -> failwith "Values are not in the same finite field"

    static member (+) (lhs : FiniteFieldElement, rhs : FiniteFieldElement) =
        FiniteFieldElement.AssertSameField (lhs, rhs)
        {lhs with Element = (lhs.Element + rhs.Element) % lhs.Modulus}

    static member (*) (lhs : FiniteFieldElement, rhs : FiniteFieldElement) =
        FiniteFieldElement.AssertSameField (lhs, rhs)
        {lhs with Element = (lhs.Element * rhs.Element) % lhs.Modulus}

    static member (*) (lhs : FiniteFieldElement, rhs : bigint) =
        {lhs with Element = (lhs.Element * rhs) % lhs.Modulus}

    member this.ToBigInt() =
        let numerator = this.Element.Numerator
        let denominator = this.Element.Denominator
        let modulus = this.Modulus

        let modInvDen = Math.modularInverse denominator modulus
        match modInvDen with
        | None -> failwithf "%s modinv %s is not valid" ((denominator).ToString()) (modulus.ToString())
        | Some mid -> numerator * mid

module FiniteFieldElement =

    let fromRational (modulus : bigint) (value : BigRational) =
        {Modulus = modulus; Element = value}

    let fromBigInt (modulus : bigint) (value : bigint) =
        {Modulus = modulus; Element = BigRational.fromBigInt value}

    let toBigInt (v : FiniteFieldElement) = v.ToBigInt()