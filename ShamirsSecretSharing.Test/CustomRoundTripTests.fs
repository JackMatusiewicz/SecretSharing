namespace ShamirsSecretSharing.Test

open NUnit.Framework
open SecretSharing
open System
open System.Text

[<TestFixture>]
module CustomRoundTripTests =

    let toBigInt (password : string) =
        password.ToCharArray ()
        |> Array.map byte
        |> bigint

    let toString (v : bigint) =
        v.ToByteArray()
        |> Array.map char
        |> Array.fold
            (fun (sb : StringBuilder) (c : char) -> sb.Append(c))
            (StringBuilder())
        |> (fun sb -> sb.ToString())

    let createSharer () =
        CustomSharer.make (Func<string, bigint> (toBigInt))

    let createReconstructor () =
        CustomReconstructor.make (Func<bigint, string> (toString))

    let generateRandomPassword (len : int) =
        let rec make (len : int) (acc : StringBuilder) (rg : RandomGenerator<int>) =
            match len with
            | 0 -> acc.ToString()
            | _ ->
                let c = ((RandomGenerator.generate rg) % 93) + 33 |> char
                make (len - 1) (acc.Append(c)) rg

        let gen = RandomGenerator.makeRandomIntGenerator ()
        make len (StringBuilder()) gen

    [<Test>]
    [<Repeat(2000)>]
    let ``Given a password and a required number of shares, when those shares are present then secret is returned`` () =
        let mySecret = generateRandomPassword 40
        let generator = createSharer ()
        let p, shares = generator.GenerateCoordinates (3u, 6u, mySecret)
        let shares = shares |> List.take 3
        let recon = createReconstructor ()
        let secret = recon.ReconstructSecret (p,shares)
        Assert.That (secret, Is.EqualTo(mySecret))

    [<Test>]
    [<Repeat(300)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then error is thrown`` () =
        let mySecret = generateRandomPassword 40
        let generator = createSharer ()
        let p, shares = generator.GenerateCoordinates (5u, 6u, mySecret)
        let shares = shares |> List.take 2
        let recon = createReconstructor ()
        let secret = recon.ReconstructSecret (p,shares)
        Assert.That (secret, Is.Not.EqualTo(mySecret))

    [<Test>]
    [<Repeat(300)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then secret is not returned`` () =
        let mySecret = generateRandomPassword 40
        let generator = createSharer ()
        let p, shares = generator.GenerateCoordinates (5u, 6u, mySecret)
        let share = shares |> List.take 1
        let recon = createReconstructor ()
        let secret = recon.ReconstructSecret (p,share)
        Assert.That (secret, Is.Not.EqualTo(mySecret))