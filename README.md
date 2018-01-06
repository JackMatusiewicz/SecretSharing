# ShamirsSecretSharing

[![Build status](https://ci.appveyor.com/api/projects/status/uow4jkvbkm9s6rk3?svg=true)](https://ci.appveyor.com/project/JackMatusiewicz/ShamirsSecretSharing)
[![Coverage Status](https://coveralls.io/repos/github/JackMatusiewicz/ShamirsSecretSharing/badge.svg?branch=master)](https://coveralls.io/github/JackMatusiewicz/ShamirsSecretSharing?branch=master)

About
-----
An F# implementation of Shamir's Secret Sharing with as few third party dependencies as possible.

Use with BigIntegers
-----

In F#, to create a new set of shares you call:
```
let generator = SecretSharer.make ()
let prime, coords = generator.GenerateCoordinates (3u, 6u, mySecret)
```

SecretSharing uses finite field arithmetic, so the prime is required for reconstructing the secret. The coords are a list of (x,y) tuples. The parameters to the GenerateSecret method are: The minimum number of coordinates required to reconstruct the secret, the number of coordinates to generate, the secret to share.

In order to reconstruct the secret, do the following:
```
let reconstructor = SecretReconstructor.make ()
let secret = reconstructor.ReconstructSecret (prime,providedCoords)
```
Where prime is the same prime number that wass generated above.

Both makeGenerator and makeReconstructor return an object implementing an interface for the task, so this is easy to use both from F# and from C#. If you're using F# you can choose to use the objects or the underlying functions directly.

Use with passwords
-----

A small wrapper has been provided to allow the use of string secrets, rather than bigIntegers, here is an example:

```
let generator = PasswordSharer.make ()
let prime, coords = generator.GenerateCoordinates (3u, 6u, "TestPassword175")

let reconstructor = PasswordReconstructor.make ()
let secret = reconstructor.ReconstructSecret (prime,providedCoords)
```

If you're using F#, you can choose to ignore the provided classes and use the functions directly.
