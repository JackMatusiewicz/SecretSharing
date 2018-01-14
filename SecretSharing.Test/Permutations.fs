namespace SecretSharing.Test

module Permutations =

    //Create all permutations of a list of things.
    let make (data : 'a list) : 'a list list =
        let rec addToLists (value : 'a) (head : 'a list) (tail : 'a list) (acc : 'a list list) =
            let newList = head @ [value] @ tail
            match tail with
            | h::t ->
                addToLists value (h::head) t (newList :: acc)
            | _ -> newList::acc

        let rec create (acc : 'a list list) (data : 'a list) : 'a list list =
            match data with
            | h::t ->
                let newAcc = List.collect (fun d -> addToLists h [] d []) acc
                create newAcc t
            | [] -> acc

        create [[]] data