namespace SecretSharing

type RandomGeneration<'t> =
    | Generator of (unit -> 't)

module RandomGeneration =

    let generate (generator : RandomGeneration<'t>) : 't =
        let (Generator rg) = generator
        rg ()