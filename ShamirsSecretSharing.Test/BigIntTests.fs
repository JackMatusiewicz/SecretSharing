namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Numerics
open Math

[<TestFixture>]
module BigIntTests =

    [<Test>]
    let ``Given a negative number, first power of two larger is 2`` () =
        let x = (bigint -1)
        let result = BigInt.findPowerOfTwoLarger x
        Assert.That(result, Is.EqualTo(bigint 2))