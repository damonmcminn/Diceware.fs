module Diceware

open System

module Util =
    let wordList =
        let lineToTuple (line:string) =
            line.Split('\t')
            |> function [|key; word|] -> key, word | _ -> failwith "Shutting up the warning"

        IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
        |> sprintf "%s/eff_large_wordlist.txt"
        |> System.IO.File.ReadLines
        |> Seq.map lineToTuple
        |> Map.ofSeq

    let generatePhrase rolls =
        rolls
        |> Seq.map (fun key -> Map.find key wordList)
        |> fun words -> String.Join(" ", words)

    let parseCliArgs argv =
        match Array.tryHead argv with
        | None -> 4
        | Some i ->
            match Int32.TryParse i with
            | _, 0 -> 4
            | _, n -> n

module Dice =
    let singleRoll (rand:Random) =
        seq { for i in 1..5 -> rand.Next(1, 6).ToString()}
        |> String.Concat

    let rollDice num =
        let rand = new Random()
        seq { for _ in 1..num -> singleRoll rand }

[<EntryPoint>]
let main argv =
    let numWords = Util.parseCliArgs argv

    printfn "%s" ((Dice.rollDice >> Util.generatePhrase) numWords)
    0 // return an integer exit code
