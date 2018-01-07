# ShamirsSecretSharing

[![Build status](https://ci.appveyor.com/api/projects/status/uow4jkvbkm9s6rk3?svg=true)](https://ci.appveyor.com/project/JackMatusiewicz/SecretSharing)
[![Coverage Status](https://coveralls.io/repos/github/JackMatusiewicz/SecretSharing/badge.svg?branch=master)](https://coveralls.io/github/JackMatusiewicz/SecretSharing?branch=master)

About
-----
An F# implementation of Shamir's Secret Sharing with as few third party dependencies as possible.

Use with BigIntegers
-----

In F#, to create a new set of shares you call:
```fsharp
let generator = SecretSharer.make ()
let prime, coords = generator.GenerateCoordinates (3u, 6u, mySecret)
```

SecretSharing uses finite field arithmetic, so the prime is required for reconstructing the secret. The coords are a list of (x,y) tuples. The parameters to the GenerateSecret method are: The minimum number of coordinates required to reconstruct the secret, the number of coordinates to generate, the secret to share.

In order to reconstruct the secret, do the following:
```fsharp
let reconstructor = SecretReconstructor.make ()
let secret = reconstructor.ReconstructSecret (prime,providedCoords)
```
Where prime is the same prime number that wass generated above.

Both makeGenerator and makeReconstructor return an object implementing an interface for the task, so this is easy to use both from F# and from C#. If you're using F# you can choose to use the objects or the underlying functions directly.

Use with passwords
-----

In order to use this library for anything other than bigIntegers, use the CustomSharer and CustomReconstructor. These allow you to pass
functions that will deal with custom types. Here is an example:

```fsharp
let toBigInt (password : string) =
    password.ToCharArray ()
    |> Array.map byte
    |> bigint

let toString (v : bigint) =
    v.ToByteArray()
    |> Array.map char
    |> Array.fold
        (fun (sb : StringBuilder) (c : char) -> sb.Append(c))
        (StringBuilder())
    |> (fun sb -> sb.ToString())

let id = (Func<Coordinate, Coordinate> (fun a -> a))

let sharer = CustomSharer.make ((Func<string, bigint> toBigInt), id)

let reconstructor = CustomReconstructor.make ((Func<bigint, string> toString), id)

sharer.GenerateCoordinates (3u, 6u, "TestPassword")
|> fun (p, coords) -> (p, coords |> List.take 3)
|> reconstructor.ReconstructSecret
```
