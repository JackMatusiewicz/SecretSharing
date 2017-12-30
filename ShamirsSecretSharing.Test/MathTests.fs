namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Numerics
open Math

module MathTests =

    let gcd (a : bigint) (b : bigint) =
        BigInteger.GreatestCommonDivisor(a,b)

    let numGenerator = RandomGeneration.makeRandomBigintGenerator 3
    let prime = (bigint 65537)

    [<Test>]
    let ``Given random modulus and number, when calculating modular inverse then result is correct``() =
        for i in 1 .. 200 do
            let a = RandomGeneration.generate numGenerator
            let modInv = Math.modularInverse a prime
            match modInv with
            | Some mi ->
                Assert.That((a * mi) % prime, Is.EqualTo(bigint 1))
            | None -> Assert.Fail("Two primes should have an inverse")

    [<Test>]
    [<Repeat(50)>]
    let ``Given two numbers, m and n, the gcd multiplied by lcm equals m * n``() =
        let makeTuple a b = a,b
        let a = (RandomGeneration.generate numGenerator)
        let b = (RandomGeneration.generate numGenerator)

        Assert.That((gcd a b) * (lcm a b), Is.EqualTo(a * b))

    [<Test>]
    [<Repeat(50)>]
    let ``Given two numbers, m and n, if there is a common factor then modInverse returns none``() =
        let a = (RandomGeneration.generate numGenerator)
        let b = (RandomGeneration.generate numGenerator)

        Assert.That(Math.modularInverse a (a * b), Is.EqualTo(None))

    [<Test>]
    [<Repeat(30)>]
    let ``Given three numbers, a,b,c then lcm (a.b, a.c) = a . lcm(b,c)``() =
        let a = (RandomGeneration.generate numGenerator)
        let b = (RandomGeneration.generate numGenerator)
        let c = (RandomGeneration.generate numGenerator)

        Assert.That(lcm (a*b) (a*c), Is.EqualTo(a * (lcm b c)))