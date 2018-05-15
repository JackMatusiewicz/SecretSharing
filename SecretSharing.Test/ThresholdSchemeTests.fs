namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System
open System.Text
open Function

[<TestFixture>]
module ThresholdSchemeTests =

    [<Test>]
        let ``Given valid values, when creating a threshold scheme, then scheme is returned`` () =
            let a = 7u
            let b = 9u

            let scheme = ThresholdScheme.make (b, a)
            Assert.That(ThresholdScheme.numberOfSharesForRecovery scheme, Is.EqualTo(7u))
            Assert.That(ThresholdScheme.numberOfSharesToMake scheme, Is.EqualTo(9u))

    [<Test>]
    let ``Given invalid values, when creating a threshold scheme, then error is thrown`` () =
        let a = 7u
        let b = 9u

        Assert.Throws<InvalidThresholdScheme>(fun () -> ThresholdScheme.make (a, b) |> ignore)
        |> ignore