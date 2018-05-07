namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Numerics

[<TestFixture>]
module BigIntTests =

    [<Test>]
    let ``Given a random value, when finding a larger mersenne prime then the correct value is returned`` () =
        let prime = BigInt.findLargerMersennePrime (bigint 1)
        Assert.That(prime, Is.EqualTo (bigint 3))

    [<Test>]
    let ``Given two elements from finite fields, when asserted to be in the same field, then exception is thrown`` () =
        let one = FiniteFieldElement.fromBigInt (bigint 5) (bigint 7)
        let two = FiniteFieldElement.fromBigInt (bigint 11) (bigint 13)

        Assert.Throws<System.Exception>(fun () -> FiniteFieldElement.AssertSameField (one, two))
        |> ignore