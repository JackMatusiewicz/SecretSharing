namespace SecretSharing

module Reader =

    let map (f : 'a -> 'b) (x : 'r -> 'a) : 'r -> 'b =
        x >> f
    let (<!>) = map

    let apply (f : 'r -> 'a -> 'b) (x : 'r -> 'a) : 'r -> 'b =
        fun r -> f r (x r)
    let (<*>) = apply