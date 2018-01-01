namespace SecretSharing

module BigInt =

    let findPowerOfTwoLarger (value : bigint) =
        let rec calculate (acc : bigint) =
            match acc >= value with
            | true -> acc
            | false -> calculate (acc <<< 1)
        calculate (bigint 2)