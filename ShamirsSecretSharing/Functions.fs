namespace SecretSharing

module Functions =

    let flip (f : 'a -> 'b -> 'c) =
        fun b a -> f a b