namespace SecretSharing

type ThresholdScheme =
    private {
        NumberOfSharesToMake : uint32
        NumberOfSharesForRecovery : uint32
    }

///Only used by the C# facing side.
exception InvalidThresholdScheme

module ThresholdScheme =

    let tryMake numberOfSharesToMake numberOfSharesForRecovery =
        match numberOfSharesToMake < numberOfSharesForRecovery with
        | true -> None
        | false ->
            {
                NumberOfSharesForRecovery = numberOfSharesForRecovery
                NumberOfSharesToMake = numberOfSharesToMake
            } |> Some

    [<CompiledName("Make")>]
    let make (numberOfSharesToMake, numberOfSharesForRecovery) =
        match tryMake numberOfSharesToMake numberOfSharesForRecovery with
        | Some s -> s
        | None -> raise InvalidThresholdScheme

    let numberOfSharesToMake (ts : ThresholdScheme) = ts.NumberOfSharesToMake

    let numberOfSharesForRecovery (ts : ThresholdScheme) = ts.NumberOfSharesForRecovery