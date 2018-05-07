namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open Function

[<TestFixture>]
module RoundTripTests =

    let take (amount : int) (a : System.Collections.Generic.List<'a>) =
        a
        |> List.ofSeq
        |> List.take amount
        |> toGenericList

    [<Test>]
    [<Repeat(50)>]
    let ``Given a secret and a required number of shares, when those shares are present then secret is returned`` () =
        let gen = RandomGenerator.makeRandomBigintGenerator 40
        let mySecret = RandomGenerator.generate gen
        let generator = SecretSharer.make()
        let ret = generator.GenerateCoordinates (3u, 6u, mySecret)
        let allSharePermutations = Permutations.ofLength 3 (List.ofSeq ret.Shares)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = SecretReconstructor.make ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.EqualTo(mySecret))
    
    [<Test>]
    [<Repeat(50)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then error is thrown`` () =
        let gen = RandomGenerator.makeRandomBigintGenerator 4
        let mySecret = RandomGenerator.generate gen
        let generator = SecretSharer.make()
        let ret = generator.GenerateCoordinates (5u, 6u, mySecret)
        let allSharePermutations = Permutations.ofLength 2 (List.ofSeq ret.Shares)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = SecretReconstructor.make ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.Not.EqualTo(mySecret))

    [<Test>]
    [<Repeat(50)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then secret is not returned`` () =
        let gen = RandomGenerator.makeRandomBigintGenerator 4
        let mySecret = RandomGenerator.generate gen
        let generator = SecretSharer.make ()
        let ret = generator.GenerateCoordinates (5u, 6u, mySecret)
        let allSharePermutations = Permutations.ofLength 1 (List.ofSeq ret.Shares)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = SecretReconstructor.make ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.Not.EqualTo(mySecret))

    [<Test>]
    let ``Given shares, when reconstructed, then secret is correct`` () =
        let secret = (bigint 5538956)
        let prime = (bigint 2147483647)
        let c1 = {X = (bigint 198773898); Y = (bigint 1215122073)}
        let c2 = {X = (bigint 511737190); Y = (bigint 1566900739)}
        let reconstructor = SecretReconstructor.make ()
        let result = reconstructor.ReconstructSecret (prime, Function.toGenericList [c1;c2])
        Assert.That(result, Is.EqualTo(secret))