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
    [<Repeat(5)>]
    let ``Given a secret and a required number of shares, when those shares are present then secret is returned`` () =
        let gen = RandomGenerator.makeRandomBigintGenerator 40
        let mySecret = RandomGenerator.generate gen
        let generator = SecretSharer.make()
        let ret = generator.GenerateCoordinates (3u, 6u, mySecret)
        let allSharePermutations = Permutations.make (List.ofSeq ret.Shares) |> List.map (List.take 3)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = SecretReconstructor.make ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.EqualTo(mySecret))
    
    [<Test>]
    [<Repeat(5)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then error is thrown`` () =
        let gen = RandomGenerator.makeRandomBigintGenerator 4
        let mySecret = RandomGenerator.generate gen
        let generator = SecretSharer.make()
        let ret = generator.GenerateCoordinates (5u, 6u, mySecret)
        let allSharePermutations = Permutations.make (List.ofSeq ret.Shares) |> List.map (List.take 2)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = SecretReconstructor.make ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.Not.EqualTo(mySecret))

    [<Test>]
    [<Repeat(5)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then secret is not returned`` () =
        let gen = RandomGenerator.makeRandomBigintGenerator 4
        let mySecret = RandomGenerator.generate gen
        let generator = SecretSharer.make()
        let ret = generator.GenerateCoordinates (5u, 6u, mySecret)
        let allSharePermutations = Permutations.make (List.ofSeq ret.Shares) |> List.map (List.take 1)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = SecretReconstructor.make ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.Not.EqualTo(mySecret))
