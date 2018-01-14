namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Numerics

[<TestFixture>]
module BigRationalTests =

    [<Test>]
    let ``Given a big rational, when it is subtracted from itself then result is zero`` () =
        let r = System.Random()
        for i in 0 .. 3000 do
            let a = BigRational.fromFraction (bigint  (r.Next())) (bigint  (r.Next()))
            let c = a - a
            Assert.That(c.Numerator, Is.EqualTo (bigint 0))

    [<Test>]
    let ``Ensure adding the same rational to itself is the same as scalar multiplication by two`` () =
        let r = System.Random()
        for i in 0 .. 3000 do
            let a = BigRational.fromFraction (bigint  (r.Next())) (bigint  (r.Next()))
            let c = a + a
            let b = a * (bigint 2)
            Assert.That(c.Numerator, Is.EqualTo (b.Numerator))
            Assert.That(c.Denominator, Is.EqualTo (b.Denominator))

    [<Test>]
    let ``Ensure dividing a rational with itself results in 1`` () =
        let r = System.Random()
        for i in 0 .. 3000 do
            let a = BigRational.fromFraction (bigint  (r.Next())) (bigint  (r.Next()))
            let c = a / a
            Assert.That(c.Numerator, Is.EqualTo (c.Denominator))

    [<Test>]
    let ``Ensure Multiply and divide cancel eachother out`` () =
        let r = System.Random()
        for i in 0 .. 3000 do
            let a = BigRational.fromFraction (bigint  (r.Next())) (bigint  (r.Next()))
            let c = (a * a) / a
            let result = a - c
            Assert.That(result.Numerator, Is.EqualTo(bigint 0))