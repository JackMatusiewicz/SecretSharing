namespace Sham

type Result<'f,'s> =
    | Failure of 'f
    | Success of 's

module Result =
    
    let map (f : 'a -> 'b) (r : Result<'f,'a>) : Result<'f,'b> =
        match r with
        | Failure f -> Failure f
        | Success x -> x |> f |> Success
    let (<!>) = map
    
    let apply (f : Result<'f, 'a -> 'b>) (x : Result<'f,'a>) : Result<'f,'b> =
        match f,x with
        | (Success f, Success x) -> x |> f |> Success
        | Failure f, _
        | _, Failure f -> Failure f