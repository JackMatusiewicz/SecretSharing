namespace SecretSharing

module Reader =

    let map (f : 'a -> 'b) (x : 'r -> 'a) : 'r -> 'b =
        fun a -> a |> x |> f
    let (<?>) x f = map f x

    let apply (f : 'r -> 'a -> 'b) (x : 'r -> 'a) : 'r -> 'b =
        fun r -> f r (x r)
    let (<*>) = apply