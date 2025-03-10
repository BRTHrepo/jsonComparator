module ConsoleApp1.Json2

open System
open System.IO
open System.Text.Json
open System.Linq
open System.Text.Json.Nodes
// JSON adatok típusa
type JsonData = JsonElement

// Függvény a JSON fájlok olvasására
let readJsonFile (filePath: string) =
    try
        File.ReadAllText(filePath) |> JsonDocument.Parse
    with ex ->
        failwithf "Hiba a fájl olvasása során: %s" ex.Message

// Rekurzív függvény a JSON objektumok tulajdonságainak rendezésére
let rec sortJsonProperties (json: JsonElement) : JsonElement =
    match json.ValueKind with
    | JsonValueKind.Object ->
        let sortedProps =
            json.EnumerateObject()
            |> Seq.sortBy (fun p -> p.Name)
            |> Seq.map (fun p ->
                let value =
                    match p.Value.ValueKind with
                    | JsonValueKind.Object -> sortJsonProperties p.Value
                    | JsonValueKind.Array -> sortJsonArray p.Value
                    | _ -> p.Value

                p.Name, value)
            |> dict

        let options = JsonSerializerOptions()
        let sortedJsonString = JsonSerializer.Serialize(sortedProps, options)
        JsonDocument.Parse(sortedJsonString).RootElement
    | JsonValueKind.Array -> sortJsonArray json
    | _ -> json

// Array típusú JSON elemek rendezése
and sortJsonArray (json: JsonElement) : JsonElement =
    let sortedArray = json.EnumerateArray() |> Seq.map sortJsonProperties |> Seq.toArray
    let options = JsonSerializerOptions()
    let sortedJsonString = JsonSerializer.Serialize(sortedArray, options)
    JsonDocument.Parse(sortedJsonString).RootElement


let compareJson (json1: JsonElement) (json2: JsonElement) : string list =
    let rec compare (j1: JsonElement) (j2: JsonElement) : string list =
        match j1.ValueKind, j2.ValueKind with
        | JsonValueKind.Object, JsonValueKind.Object ->
            let props1 = j1.EnumerateObject() |> Seq.map (fun p -> p.Name) |> Set.ofSeq
            let props2 = j2.EnumerateObject() |> Seq.map (fun p -> p.Name) |> Set.ofSeq

            if props1 <> props2 then
                [ $"""Különböző kulcsok: {Set.difference props1 props2}""" ]
            else
                props1
                |> Seq.collect (fun (p: string) ->
                    compare (j1.GetProperty(p)) (j2.GetProperty(p))
                    |> List.map (fun msg -> $"Kulcs: {p}, Eltérés: {msg}")
                )
                |> List.ofSeq
        | JsonValueKind.Array, JsonValueKind.Array ->
            let arr1 = j1.EnumerateArray() |> Seq.toArray
            let arr2 = j2.EnumerateArray() |> Seq.toArray

            if arr1.Length <> arr2.Length then
                [ $"""Különböző tömbméretek: {arr1.Length} vs {arr2.Length}""" ]
            else
                Seq.zip arr1 arr2
                |> Seq.mapi (fun i (a1, a2) ->
                    compare a1 a2
                    |> List.map (fun msg -> $"[{i}]: {msg}")
                )
                |> Seq.collect id
                |> List.ofSeq
        | _, _ when j1.ValueKind <> j2.ValueKind || j1.GetRawText() <> j2.GetRawText() ->
            let value1 =
                match j1.ValueKind with
                | JsonValueKind.String -> j1.GetString()
                | JsonValueKind.Number -> j1.GetDouble().ToString()
                | JsonValueKind.True | JsonValueKind.False -> j1.GetBoolean().ToString()
                | JsonValueKind.Null -> "null"
                | _ -> j1.GetRawText()
            let value2 =
                match j2.ValueKind with
                | JsonValueKind.String -> j2.GetString()
                | JsonValueKind.Number -> j2.GetDouble().ToString()
                | JsonValueKind.True | JsonValueKind.False -> j2.GetBoolean().ToString()
                | JsonValueKind.Null -> "null"
                | _ -> j2.GetRawText()
            [ $"""Különböző értékek: {value1} vs {value2}""" ]
        | _ -> []

    compare json1 json2


// Függvény a JSON fájlok összehasonlítására
let compareJsonFiles (file1Path: string) (file2Path: string) : string list =
    let json1 = readJsonFile file1Path
    let json2 = readJsonFile file2Path
    let sortedJson1 = sortJsonProperties json1.RootElement
    let sortedJson2 = sortJsonProperties json2.RootElement

    // Az összehasonlítás eredménye lista formában
    let differences = compareJson sortedJson1 sortedJson2

    if List.isEmpty differences then
        [ "Azonos JSON adatok" ]
    else
        differences

// Példa használat
let file1Path = "a.json"
let file2Path = "b.json"
let result = compareJsonFiles file1Path file2Path
printfn "json2.fs -----------------------------------------------------------------"
//printfn "%s" result
