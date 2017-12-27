namespace SecretSharing

open System
open System.Numerics
open System.Security.Cryptography

type RandomGeneration<'t> =
    | Generator of (unit -> 't) * IDisposable

module RandomGeneration =

    let generate (generator : RandomGeneration<'t>) : 't =
        let (Generator (rg,_)) = generator
        rg ()

    ///Generates a random positive integer
    let makeRandomIntGenerator () =
        let r = new RNGCryptoServiceProvider()
        let f = fun () ->
                    let store = Array.create 4 (byte 0)
                    r.GetBytes(store)
                    let value = BitConverter.ToInt32(store,0)
                    value &&& (0x7FFFFFFF)
        Generator (f,r)

    ///Generates a random positive big integer, of the required size.
    let makeRandomBigintGenerator (bytesInBigint : int) =
        let r = new RNGCryptoServiceProvider()
        let f = fun () ->
                    let store = Array.create bytesInBigint (byte 0)
                    store.[bytesInBigint - 1] <- (store.[bytesInBigint - 1] &&& (byte 0x7F))
                    BigInteger(store)
        Generator (f,r)