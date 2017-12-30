namespace ShamirsSecretSharing.Test

open NUnit.Framework
open SecretSharing
open System.Collections.Generic

[<TestFixture>]
module RandomGeneratorTests =

    [<Test>]
    let ``Given a random int generator, when asked to generate 100 numbers then all are unique`` () =
        let generator = RandomGeneration.makeRandomIntGenerator ()
        let values = Array.init 10 (fun _ -> RandomGeneration.generate generator)
        let store = HashSet<int>()
        Array.fold (fun (store : HashSet<int>) v ->
                        if store.Contains(v) then
                            Assert.Fail ()
                            store
                        else
                            store.Add(v) |> ignore; store) store values |> ignore

    [<Test>]
    let ``Given a random bigint generator, when asked to generate 100 numbers then all are unique`` () =
        let generator = RandomGeneration.makeRandomBigintGenerator (8)
        let values = Array.init 10 (fun _ -> RandomGeneration.generate generator)
        let store = HashSet<bigint>()
        Array.fold (fun (store : HashSet<bigint>) v ->
                        if store.Contains(v) then
                            Assert.Fail ()
                            store
                        else
                            store.Add(v) |> ignore; store) store values |> ignore

    [<Test>]
    let ``Given an int generator, when ints are generated then all are greater than or equal to 0`` () =
        let generator = RandomGeneration.makeRandomIntGenerator ()
        Array.init 100 (fun _ -> RandomGeneration.generate generator)
        |> Array.iter (fun i -> if i < 0 then Assert.Fail())

    [<Test>]
    let ``Given an bigint generator, when ints are generated then all are greater than or equal to 0`` () =
        let generator = RandomGeneration.makeRandomBigintGenerator 10
        Array.init 100 (fun _ -> RandomGeneration.generate generator)
        |> Array.iter (fun i -> if i < (bigint 0) then Assert.Fail())