namespace SecretSharing

type FiniteFieldElement = private {
    Modulus : bigint
    Element : bigint
}

module FiniteField =

    let create (modulus : bigint) (ele : bigint) =
        let fieldElement = ele % modulus
        {Modulus = modulus; Element = fieldElement}

    let element (ffe : FiniteFieldElement) = ffe.Element
    let modulus (ffe : FiniteFieldElement) = ffe.Modulus

    let assertInSameField (lhs : FiniteFieldElement) (rhs : FiniteFieldElement) =
        match lhs.Modulus = rhs.Modulus with
        | true -> ()
        | false -> failwithf "%s is not the same modulus as %s" (lhs.Modulus.ToString()) (rhs.Modulus.ToString())

    let add (lhs : FiniteFieldElement) (rhs : FiniteFieldElement) =
        assertInSameField lhs rhs
        {Modulus = lhs.Modulus; Element = (lhs.Element + rhs.Element) % lhs.Modulus}

    let sub (lhs : FiniteFieldElement) (rhs : FiniteFieldElement) =
        assertInSameField lhs rhs
        {Modulus = lhs.Modulus; Element = (lhs.Element - rhs.Element) % lhs.Modulus}

    let multiply (lhs : FiniteFieldElement) (rhs : FiniteFieldElement) =
        assertInSameField lhs rhs
        {Modulus = lhs.Modulus; Element = (lhs.Element * rhs.Element) % lhs.Modulus}

    let divide (numerator : FiniteFieldElement) (denominator : FiniteFieldElement) =
        assertInSameField numerator denominator
        let denomInverse = Math.modularInverse (denominator.Element) (denominator.Modulus)
        match denomInverse with
        | None -> failwith "Couldn't find a valid inverse"
        | Some di ->
            {Modulus = numerator.Modulus; Element = ((numerator.Element) * di) % numerator.Modulus}