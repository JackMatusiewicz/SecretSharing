﻿namespace SecretSharing.Test

open NUnit.Framework
open SecretSharing
open System
open System.Text
open Function

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

    let id = (Func<Coordinate, Coordinate> (fun a -> a))

    let createSharer () =
        CustomSharer.make ((Func<string, bigint> toBigInt), id)

    let createReconstructor () =
        CustomReconstructor.make ((Func<bigint, string> toString), id)

    let generateRandomPassword (len : int) =
        let rec make (len : int) (acc : StringBuilder) (rg : RandomGenerator<int>) =
            match len with
            | 0 -> acc.ToString()
            | _ ->
                let c = ((RandomGenerator.generate rg) % 93) + 33 |> char
                make (len - 1) (acc.Append(c)) rg

        let gen = RandomGenerator.makeRandomIntGenerator ()
        make len (StringBuilder()) gen
    
    let take (amount : int) (a : System.Collections.Generic.List<'a>) =
        a
        |> List.ofSeq
        |> List.take amount
        |> toGenericList

    [<Test>]
    [<Repeat(5)>]
    let ``Given a password and a required number of shares, when those shares are present then secret is returned`` () =
        let mySecret = generateRandomPassword 40
        let generator = createSharer ()
        let ret = generator.GenerateCoordinates (3u, 6u, mySecret)
        let allSharePermutations = Permutations.make (List.ofSeq ret.Shares) |> List.map (List.take 3)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = createReconstructor ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.EqualTo(mySecret))

    [<Test>]
    [<Repeat(5)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then error is thrown`` () =
        let mySecret = generateRandomPassword 40
        let generator = createSharer ()
        let ret = generator.GenerateCoordinates (5u, 6u, mySecret)
        let allSharePermutations = Permutations.make (List.ofSeq ret.Shares) |> List.map (List.take 2)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = createReconstructor ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.Not.EqualTo(mySecret))

    [<Test>]
    [<Repeat(5)>]
    let ``Given a secret and a required number of shares, when not enough shares are present then secret is not returned`` () =
        let mySecret = generateRandomPassword 40
        let generator = createSharer ()
        let ret = generator.GenerateCoordinates (5u, 6u, mySecret)
        let allSharePermutations = Permutations.make (List.ofSeq ret.Shares) |> List.map (List.take 3)
        let shares = List.map (Function.toGenericList) allSharePermutations
        let recon = createReconstructor ()
        for share in shares do
            let secret = recon.ReconstructSecret (ret.Prime,share)
            Assert.That (secret, Is.Not.EqualTo(mySecret))
