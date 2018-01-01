namespace SecretSharing

open System
open System.Numerics
open System.Security.Cryptography

type RandomGenerator<'t> =
    | Generator of (unit -> 't)

module RandomGeneration =

    let generate (generator : RandomGenerator<'t>) : 't =
        let (Generator (rg)) = generator
        rg ()

    ///Generates a random positive integer
    let makeRandomIntGenerator () =
        let r = new RNGCryptoServiceProvider()
        let f = fun () ->
                    let store = Array.create 4 (byte 0)
                    r.GetBytes(store)
                    let value = BitConverter.ToInt32(store,0)
                    (value &&& (0x7FFFFFFF))
        Generator f

    ///Generates a random positive big integer, of the required size.
    let makeRandomBigintGenerator (bytesInBigint : int) =
        let r = new RNGCryptoServiceProvider()
        let f = fun () ->
                    let store = Array.create bytesInBigint (byte 0)
                    r.GetBytes(store)
                    store.[bytesInBigint - 1] <- (store.[bytesInBigint - 1] &&& (byte 0x7F))
                    BigInteger(store)
        Generator f

    ///This picks in the range of 0, (max - 1), will throw if max is negative.
    ///Will pick uniformly in the range.
    let makeRandomBigIntRange (max : bigint) =
        let maxMinusOne = max - (bigint 1)
        let findInRange (f : unit -> bigint) : unit -> bigint =
            let rec find (f : unit -> bigint) =
                let value = f ()
                match value >= (bigint 0) && value <= maxMinusOne with
                | true -> value
                | false -> find f
            fun _ -> find f

        if maxMinusOne <= (bigint 0) then
            failwith "Max must be a non-negative integer"
        let largerPowerOfTwo = BigInt.findPowerOfTwoLarger maxMinusOne
        let numberOfBytes = largerPowerOfTwo.ToByteArray().Length
        let (Generator rg) = makeRandomBigintGenerator numberOfBytes
        Generator <| findInRange rg