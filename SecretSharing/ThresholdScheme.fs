namespace SecretSharing

type ThresholdScheme =
    private {
        NumberOfSharesToMake : uint32
        NumberOfSharesForRecovery : uint32
    }

module ThresholdScheme =

    let make numberOfSharesToMake numberOfSharesForRecovery =
        match numberOfSharesToMake > numberOfSharesForRecovery with
        | true -> None
        | false ->
            {
                NumberOfSharesForRecovery = numberOfSharesForRecovery
                NumberOfSharesToMake = numberOfSharesToMake
            } |> Some

    let numberOfSharesToMake (ts : ThresholdScheme) = ts.NumberOfSharesToMake

    let numberOfSharesForRecovery (ts : ThresholdScheme) = ts.NumberOfSharesForRecovery