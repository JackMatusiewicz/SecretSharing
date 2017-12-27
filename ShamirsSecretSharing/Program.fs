open System
open System.Numerics

open SecretSharing

let test () =
    let mySecret = 11234
    let generator = SecretSharing.makeGenerator()
    let shares = generator.GenerateSecret (3u, 6u, mySecret)
    let secret = SecretSharing.getSecret 3u shares
    printfn "%d" secret

[<EntryPoint>]
let main argv =
    test ()
    0
