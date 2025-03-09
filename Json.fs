module ConsoleApp1.Json

open System.IO
open System.Text.Json
open System.Linq

// JSON adatok típusa
type JsonData = JsonDocument

// Függvény a JSON fájlok olvasására
let readJsonFile (filePath: string) =
    try
        File.ReadAllText(filePath) |> JsonDocument.Parse
    with
    | ex -> failwithf $"Hiba a fájl olvasása során: %s{ex.Message}"


// Függvény a JSON objektumok sorrendbe rendezésére
let sortJsonProperties (json: JsonElement) =
    // Enumerate and sort properties alphabetically by name
    let properties = json.EnumerateObject()
                          |> Seq.sortBy (fun p -> p.Name)

    // Convert to dictionary with string representation of the values
    let sortedDict = 
        properties
        |> Seq.map (fun p -> 
            p.Name, 
            match p.Value.ValueKind with
            | JsonValueKind.String -> p.Value.GetString()
            | JsonValueKind.Number -> p.Value.GetDouble().ToString()
            | JsonValueKind.True | JsonValueKind.False -> p.Value.GetBoolean().ToString()
            | JsonValueKind.Null -> "null"
            | JsonValueKind.Array 
            | JsonValueKind.Object -> p.Value.GetRawText()
            | _ -> failwith "Unsupported JSON type"
        )
        |> dict
    let options = JsonSerializerOptions()
    let sortedJsonString = JsonSerializer.Serialize(sortedDict, options)
    JsonDocument.Parse(sortedJsonString).RootElement
    
let compareJson (json1: JsonElement) (json2: JsonElement) =
    let rec compare (j1: JsonElement) (j2: JsonElement) =
        match j1.ValueKind, j2.ValueKind with
        | JsonValueKind.Object, JsonValueKind.Object ->
            let props1 = j1.EnumerateObject()
            let props2 = j2.EnumerateObject()
            let props1Set = set [ for p in props1 -> p.Name ]
            let props2Set = set [ for p in props2 -> p.Name ]
            if props1Set <> props2Set then
                Some "Különböző kulcsok"
            else
                let differences =
                    seq {
                        for p in props1 do
                            let prop2 = j2.GetProperty(p.Name)
                            match compare p.Value prop2 with
                            | Some msg -> yield $"Kulcs: {p.Name}, Eltérés: {msg}"
                            | None -> ()
                    }
                if Seq.isEmpty differences then None
                else Some (String.concat "; " differences)
        | JsonValueKind.Array, JsonValueKind.Array ->
            let arr1 = j1.EnumerateArray().ToArray()
            let arr2 = j2.EnumerateArray().ToArray()
            if arr1.Length <> arr2.Length then
                Some "Különböző tömbméretek"
            else
                let differences =
                    Seq.zip arr1 arr2
                    |> Seq.mapi (fun i (a1, a2) ->
                        match compare a1 a2 with
                        | Some msg -> Some $"[%d{i}]: {msg}"
                        | None -> None)
                    |> Seq.choose id
                if Seq.isEmpty differences then None
                else Some (String.concat "; " differences)
        | _, _ when j1.ToString() <> j2.ToString() -> Some "Különböző értékek"
        | _ -> None

    match compare json1 json2 with
    | Some msg -> msg
    | None -> "Azonos JSON adatok"

// Függvény a JSON fájlok összehasonlítására
let compareJsonFiles (file1Path: string) (file2Path: string) =
    let json1 = readJsonFile file1Path
    let json2 = readJsonFile file2Path
    let sortedJson1 = sortJsonProperties json1.RootElement
    let sortedJson2 = sortJsonProperties json2.RootElement
    compareJson sortedJson1 sortedJson2

// Példa használat
let file1Path =  "a.json"
let file2Path = "b.json"
let result = compareJsonFiles file1Path file2Path
printfn "json.fs -----------------------------------------------------------------"
printfn $"%s{result}"
printfn"-----------------------------------------"
printfn""


