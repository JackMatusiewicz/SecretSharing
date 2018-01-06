namespace SecretSharing

open Reader
open System
open System.Numerics
open Math

type IPasswordSharer =
    abstract member GenerateCoordinates : uint32 * uint32 * string -> Prime * Coordinate list

//Wraps the SecretSharer so you can deal with strings, rather than with bigints.
module PasswordSharer =

    let toBigInt (password : string) =
        password.ToCharArray ()
        |> Array.map byte
        |> bigint

    let make () =
        { new IPasswordSharer with
                member __.GenerateCoordinates (minimumSegmentsToSolve, numberOfCoords, secret) =
                    secret
                    |> toBigInt
                    |> SecretSharer.generateCoordinates minimumSegmentsToSolve numberOfCoords }