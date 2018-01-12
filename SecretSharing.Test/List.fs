namespace ShamirsSecretSharing.Test

module List =

    let take (num : int) (l : 't list) : 't list =
        let rec calc (acc : 't list) (l : 't list) (num : int) =
            match num with
            | _ when num <= 0 -> List.rev acc
            | _ ->
                match l with
                | [] -> List.rev acc
                | h::t ->
                    calc (h :: acc) t (num - 1)
        calc [] l num