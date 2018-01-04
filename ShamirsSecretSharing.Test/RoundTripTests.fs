namespace ShamirsSecretSharing.Test

open NUnit.Framework
open SecretSharing

[<TestFixture>]
module RoundTripTests =

    [<Test>]
    [<Repeat(2000)>]
    let ``Given a secret and a required number of shares, when those shares are present then secret is returned`` () =
        let gen = RandomGeneration.makeRandomBigintGenerator 40
        let mySecret = RandomGeneration.generate gen
        let generator = SecretSharing.makeGenerator()
        let p, shares = generator.GenerateCoordinates (3u, 6u, mySecret)
        let shares = shares |> List.take 3
        let recon = SecretSharing.makeReconstructor ()
        let secret = recon.ReconstructSecret (p,shares)
        Assert.That (secret, Is.EqualTo(mySecret))
    
    [<Test>]
    [<Repeat(300)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then error is thrown`` () =
        let gen = RandomGeneration.makeRandomBigintGenerator 4
        let mySecret = RandomGeneration.generate gen
        let generator = SecretSharing.makeGenerator()
        let p, shares = generator.GenerateCoordinates (5u, 6u, mySecret)
        let shares = shares |> List.take 2
        let recon = SecretSharing.makeReconstructor ()
        let secret = recon.ReconstructSecret (p,shares)
        Assert.That (secret, Is.Not.EqualTo(mySecret))

    [<Test>]
    [<Repeat(300)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then secret is not returned`` () =
        let gen = RandomGeneration.makeRandomBigintGenerator 4
        let mySecret = RandomGeneration.generate gen
        let generator = SecretSharing.makeGenerator()
        let p, shares = generator.GenerateCoordinates (5u, 6u, mySecret)
        let share = shares |> List.take 1
        let recon = SecretSharing.makeReconstructor ()
        let secret = recon.ReconstructSecret (p,share)
        Assert.That (secret, Is.Not.EqualTo(mySecret))