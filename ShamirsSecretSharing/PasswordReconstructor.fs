namespace SecretSharing

open Reader
open System
open System.Numerics
open System.Text
open Math

type IPasswordReconstructor =
    abstract member ReconstructSecret : bigint*Coordinate list -> string

module PasswordReconstructor =

    let toString (v : bigint) =
        v.ToByteArray()
        |> Array.map char
        |> Array.fold
            (fun (sb : StringBuilder) (c : char) -> sb.Append(c))
            (StringBuilder())
        |> (fun sb -> sb.ToString())

    let make () =
        { new IPasswordReconstructor with
                member __.ReconstructSecret (prime, coords) : string =
                    SecretReconstructor.getSecret prime coords
                    |> toString }
