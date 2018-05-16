# SecretSharing

| Build | Status |
| --- | --- |
| Linux | [![Build Status](https://travis-ci.org/JackMatusiewicz/SecretSharing.svg?branch=master)](https://travis-ci.org/JackMatusiewicz/SecretSharing) |
| Windows | [![Build status](https://ci.appveyor.com/api/projects/status/uow4jkvbkm9s6rk3?svg=true)](https://ci.appveyor.com/project/JackMatusiewicz/SecretSharing) |
| Coverage | [![Coverage Status](https://coveralls.io/repos/github/JackMatusiewicz/SecretSharing/badge.svg?branch=master)](https://coveralls.io/github/JackMatusiewicz/SecretSharing?branch=master) |
| NuGet | [![NuGet Status](http://img.shields.io/nuget/vpre/SecretSharing.svg?style=flat)](https://www.nuget.org/packages/SecretSharing/) |

About
-----
An F# implementation of Shamir's Secret Sharing with as few third party dependencies as possible.

Download
-----
This project is available as a NuGet package here: https://www.nuget.org/packages/SecretSharing/

Use with BigIntegers
-----

In C#, to create a new set of shares you call:
```csharp
var sharer = SecretSharer.Make();
var primeAndShares = sharer.GenerateCoordinates(3, 6, new BigInteger(2858295));
```

SecretSharing uses finite field arithmetic, so the prime is required for reconstructing the secret. The coords are a list of (x,y) tuples. The parameters to the GenerateSecret method are: The minimum number of coordinates required to reconstruct the secret, the number of coordinates to generate, the secret to share.

In order to reconstruct the secret, do the following:
```csharp
var reconstructor = SecretReconstructor.Make();
var number = reconstructor.ReconstructSecret(primeAndShares.Prime, usableShares);
```
Where prime is the same prime number that wass generated above.

Both makeGenerator and makeReconstructor return an object implementing an interface for the task, so this is easy to use both from F# and from C#. If you're using F# you can choose to use the objects or the underlying functions directly.

Use with passwords
-----

In order to use this library for anything other than bigIntegers, use the CustomSharer and CustomReconstructor. These allow you to pass
functions that will deal with custom types. Here is an example:

```csharp
Func<string, BigInteger> toBigInt = s =>
{
    var bytes = s
        .ToCharArray()
        .Select(a => (byte)a);
    return new BigInteger(bytes.ToArray());
};

Func<BigInteger, string> toString = bi =>
{
    var chars = bi
        .ToByteArray()
        .Select(a => (char)a)
        .ToArray();
    return new string(chars);
};

var sharer = CustomSharer.Make(toBigInt, coord => coord, prime => prime);
var primeAndShares = sharer.GenerateCoordinates(3, 6, "Hello123pass!@!_:");

var reconstructor = CustomReconstructor.Make<string, Coordinate, Prime>(toString, coord => coord, prime => prime);
var password = reconstructor.ReconstructSecret(primeAndShares.Prime, primeAndShares.Shares);
```

F# Example
-----

Using F#, you can just access the functions directly. Also note that there is a tryMake function inside ThresholdScheme that will return an option type, rather than throwing an exception if you pass incorrect data.

```fsharp
let toBigInt (s : string) =
    s.ToCharArray ()
    |> Array.map byte
    |> bigint

let toString (bi : bigint) =
    bi.ToByteArray ()
    |> Array.map char
    |> fun c -> new string (c)

let ts = ThresholdScheme.make (6u, 4u)

let (prime, coords) = 
    CustomSharer.generateCoordinates
        toBigInt
        id
        id
        ts
        "helloWorld"

let secret =
    CustomReconstructor.reconstructSecret
        toString
        id
        id
        prime
        coords
printfn "%s" secret
```
