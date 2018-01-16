namespace SecretSharing

open System.Collections.Generic

///What is returned from a SecretSharer
///The prime is the modulus of the finite field
///The shares are the individual values for each user to store.
type Shares<'coord, 'prime> = {
    Prime : 'prime
    Shares : List<'coord> }

module Shares =

    let make (p : 'prime, shares : List<'coord>) =
        {Prime = p; Shares = shares}