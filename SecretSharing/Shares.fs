namespace SecretSharing

open System.Collections.Generic

///What is returned from a SecretSharer
///The prime is the modulus of the finite field
///The shares are the individual values for each user to store.
type Shares<'a> = {
    Prime : Prime
    Shares : List<'a> }

module Shares =

    let make (p : Prime, shares : List<'a>) =
        {Prime = p; Shares = shares}