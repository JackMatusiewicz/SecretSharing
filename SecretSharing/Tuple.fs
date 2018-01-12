namespace SecretSharing

module Tuple =

    let map (f : 'a -> 'b) (c : 'c, a : 'a) : 'c * 'b =
        c, f a

    let make (a : 'a) (b : 'b) : 'a * 'b =
        a,b
