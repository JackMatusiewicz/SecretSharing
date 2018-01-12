namespace SecretSharing

open System.Numerics

module BigInt =

    ///Not a comprehensive list, will add more later
    ///Values (n) such that (2^n) - 1 is a prime.
    let mersennePrimesExponents = [
        2
        3
        5
        7
        13
        17
        19
        31
        61
        89
        107
        127
        521
        607
        1279
        2203
        2281
        3217
        4253
        4423
        9689
        9941
        11213
        19937
    ]

    let private findPowerOfTwoLargerAndSetBitPosition (value : bigint) : (int*bigint) =
        let rec calculate (acc : bigint) (setBit : int) =
            match acc >= value with
            | true -> setBit, acc
            | false -> calculate (acc <<< 1) (setBit + 1)
        calculate (bigint 2) 1

    let findPowerOfTwoLarger (value : bigint) : bigint =
        findPowerOfTwoLargerAndSetBitPosition value |> snd

    let findSetBitPositionOfLargerPowerOfTwo (value : bigint) =
        findPowerOfTwoLargerAndSetBitPosition value |> fst

    let tryFindLargerMersennePrime (value : bigint) =
        let bitPosition = findSetBitPositionOfLargerPowerOfTwo value
        mersennePrimesExponents
        |> List.filter (fun x -> x > bitPosition)
        |> List.tryHead
        |> Option.map (fun pow -> BigInteger.Pow((bigint 2), pow) - (bigint 1))

    let findLargerMersennePrime (value : bigint) =
        match tryFindLargerMersennePrime value with
        | None -> failwith "Unable to find a mersenne prime larger than this value"
        | Some p -> p