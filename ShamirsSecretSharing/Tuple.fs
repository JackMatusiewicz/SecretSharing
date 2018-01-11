namespace SecretSharing

module Tuple =

    let map (f : 'a -> 'b) (c : 'c, a : 'b) : 'c * 'b =
        c, f a