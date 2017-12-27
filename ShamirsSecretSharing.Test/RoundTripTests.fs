namespace ShamirsSecretSharing.Test

open NUnit.Framework
open SecretSharing

[<TestFixture>]
module RoundTripTests =

    [<Test>]
    let ``Given a secret and a required number of shares, when those shares are present then secret is returned`` () =
            let mySecret = 11234
            let generator = SecretSharing.makeGenerator()
            let shares = generator.GenerateSecret (3u, 6u, mySecret) |> List.take 3
            let secret = SecretSharing.getSecret 3u shares
            Assert.That (secret, Is.EqualTo(mySecret))
    
    [<Test>]
    let ``Given a secret and a required number of shares, when not enough shares are present then error is thrown`` () =
            let mySecret = 11234
            let generator = SecretSharing.makeGenerator()
            let shares = generator.GenerateSecret (3u, 6u, mySecret) |> List.take 2
            Assert.Throws<System.Exception>(fun () -> SecretSharing.getSecret 3u shares |> ignore) |> ignore

    [<Test>]
    let ``Given a secret and a required number of shares, when not enough shares are present then secret is not returned`` () =
            let mySecret = 11234
            let generator = SecretSharing.makeGenerator()
            let shares = generator.GenerateSecret (3u, 6u, mySecret) |> List.take 2
            let secret = SecretSharing.getSecret 2u shares
            Assert.That (secret, Is.Not.EqualTo(mySecret))