namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Numerics
open Math

[<TestFixture>]
module BigIntTests =

    [<Test>]
    let ``Given a random value, when finding a larger mersenne prime then the correct value is returned`` () =
        let prime = BigInt.findLargerMersennePrime (bigint 1)
        Assert.That(prime, Is.EqualTo(bigint 3))