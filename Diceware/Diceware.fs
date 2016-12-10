module Diceware

open System

module Util =
    let words =
        let lineToTuple (line:string) =
            line.Split('\t')
            // http://stackoverflow.com/a/2197433
            |> function [|key; word|] -> key, word | _ -> failwith "Shutting up the warning"
        
        // http://stackoverflow.com/a/17191676
        IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
        |> sprintf "%s/eff_large_wordlist.txt"
        |> System.IO.File.ReadLines
        |> Seq.map lineToTuple
        |> Map.ofSeq

    let getPhraseLength argv =
        let defaultPhraseLength = 4

        match Array.tryHead argv with
        | None -> defaultPhraseLength
        | Some arg ->
            match Int32.TryParse arg with
            | true, i -> i
            | _ -> defaultPhraseLength

module Dice =
    let rollOnce (rand:Random) =
        seq { for _ in 1..5 -> rand.Next(1, 6).ToString() }
        |> String.Concat

    let rollNumberOfTimes num =
        let rand = new Random()
        seq { for _ in 1..num -> rollOnce rand }

let generatePhrase rolls =
    rolls
    |> Seq.map (fun key -> Map.find key Util.words)
    |> fun words -> String.Join(" ", words)

[<EntryPoint>]
let main argv =
    Util.getPhraseLength argv
    |> (Dice.rollNumberOfTimes >> generatePhrase)
    |> printfn "%s"

    0 // return an integer exit code
