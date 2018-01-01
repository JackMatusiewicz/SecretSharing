namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Numerics
open Math

[<TestFixture>]
module BigRationalTests =

    [<Test>]
    let ``Given a big rational, when it is subtracted from itself then result is zero`` () =
        let r = System.Random()
        for i in 0 .. 3000 do
            let a = BigRational.fromFraction (bigint  (r.Next())) (bigint  (r.Next()))
            let c = BigRational.sub a a
            Assert.That(c.Numerator, Is.EqualTo (bigint 0))

    [<Test>]
    let ``Ensure adding the same rational to itself is the same as scalar multiplication by two`` () =
        let r = System.Random()
        for i in 0 .. 3000 do
            let a = BigRational.fromFraction (bigint  (r.Next())) (bigint  (r.Next()))
            let c = BigRational.add a a
            let b = BigRational.scalarMultiply (bigint 2) (a)
            Assert.That(c.Numerator, Is.EqualTo (b.Numerator))
            Assert.That(c.Denominator, Is.EqualTo (b.Denominator))

    [<Test>]
    let ``Ensure dividing a rational with itself results in 1`` () =
        let r = System.Random()
        for i in 0 .. 3000 do
            let a = BigRational.fromFraction (bigint  (r.Next())) (bigint  (r.Next()))
            let c = BigRational.divide a a
            Assert.That(c.Numerator, Is.EqualTo (c.Denominator))