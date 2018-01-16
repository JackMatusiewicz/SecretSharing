namespace SecretSharing

module Tuple =

    let map (f : 'a -> 'b) (c : 'c, a : 'a) : 'c * 'b =
        c, f a

    let make (a : 'a) (b : 'b) : 'a * 'b =
        a,b

    let bimap (f : 'a -> 'b) (g : 'c -> 'd) (a,c) =
        (f a), (g c)

    let leftMap (f : 'a -> 'b) t =
        bimap f (fun x -> x) t