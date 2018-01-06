namespace SecretSharing

open System
open System.Numerics
open System.Text

type IPasswordReconstructor =
    abstract member ReconstructSecret : bigint * Coordinate list -> string

//Wraps the SecretReconstructor so you can deal with strings, rather than with bigints.
module PasswordReconstructor =

    let toString (v : bigint) =
        v.ToByteArray()
        |> Array.map char
        |> Array.fold
            (fun (sb : StringBuilder) (c : char) -> sb.Append(c))
            (StringBuilder())
        |> (fun sb -> sb.ToString())

    let getPassword (prime : bigint) (coords : Coordinate list) =
        SecretReconstructor.getSecret prime coords
        |> toString

    let make () =
        { new IPasswordReconstructor with
                member __.ReconstructSecret (prime, coords) : string =
                    getPassword prime coords }
