namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Numerics
open Math

[<TestFixture>]
module MathTests =

    let gcd (a : bigint) (b : bigint) =
        BigInteger.GreatestCommonDivisor(a,b)

    let numGenerator = RandomGenerator.makeRandomBigintGenerator 3
    let prime = (bigint 65537)

    [<Test>]
    let ``The LCM of two zeroes is zero`` () =
        Assert.That(Math.lcm (bigint 0) (bigint 0), Is.EqualTo(bigint 0))

    [<Test>]
    let ``Given random modulus and number, when calculating modular inverse then result is correct``() =
        for i in 1 .. 200 do
            let a = RandomGenerator.generate numGenerator
            let modInv = Math.modularInverse a prime
            match modInv with
            | Some mi ->
                Assert.That((a * mi) % prime, Is.EqualTo(bigint 1))
            | None -> Assert.Fail("Two primes should have an inverse")

    [<Test>]
    [<Repeat(50)>]
    let ``Given two numbers, m and n, the gcd multiplied by lcm equals m * n``() =
        let a = (RandomGenerator.generate numGenerator)
        let b = (RandomGenerator.generate numGenerator)

        Assert.That((gcd a b) * (lcm a b), Is.EqualTo(a * b))

    [<Test>]
    [<Repeat(50)>]
    let ``Given two numbers, m and n, if there is a common factor then modInverse returns none``() =
        let a = (RandomGenerator.generate numGenerator)
        let b = (RandomGenerator.generate numGenerator)

        Assert.That(Math.modularInverse a (a * b), Is.EqualTo(None))

    [<Test>]
    [<Repeat(30)>]
    let ``Given three numbers, a,b,c then lcm (a.b, a.c) = a . lcm(b,c)``() =
        let a = (RandomGenerator.generate numGenerator)
        let b = (RandomGenerator.generate numGenerator)
        let c = (RandomGenerator.generate numGenerator)

        Assert.That(lcm (a*b) (a*c), Is.EqualTo(a * (lcm b c)))