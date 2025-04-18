﻿module ConsoleApp1.Program

open System


type IPrintable =
    abstract member Print : unit -> unit

type Shape =
    | Circle of float
    | Square of float
    | Rectangle of float * float
    member this.Area =
        match this with
        | Circle r -> Math.PI * r * r
        | Square s -> s * s
        | Rectangle (l, w) -> l * w
    interface IPrintable with
         member this.Print() =
            match this with
            | Circle r -> 
                printfn "Kör sugara: %f" r
                printfn "Kör területe: %f" this.Area
            | Square s -> 
                printfn "Négyzet oldala: %f" s
                printfn "Négyzet területe: %f" this.Area
            | Rectangle (l, w) -> 
                printfn "Téglalap: %f x %f" l w
                printfn "Téglalap területe: %f" this.Area

// Példák a használatra:
let circle = Circle 5.0
let square = Square 4.0
let rectangle = Rectangle(3.0, 6.0)

// Kiíratás és terület számítás az IPrintable interfész használatával
(circle :> IPrintable).Print()
printfn ""
(square :> IPrintable).Print()
printfn ""
(rectangle :> IPrintable).Print()



[<EntryPoint>]
let main argv =
    printfn "JSON összehasonlítás indítása..."
    let file1Path = "a.json"
    let file2Path = "b.json"
    let result = Json.compareJsonFiles file1Path file2Path
    printfn $"Eredmény: %s{result}"
    
    let result2 = ConsoleApp1.Json2.compareJsonFiles file1Path file2Path
    result2 |> List.iter (printfn "%s")
    let json1 = Json.readJsonFile file1Path
    let json2 = Json.readJsonFile file2Path
    // Bejárás és Map létrehozása
    let mapJson1 = JsonTraverser.traverseJsonDocument (json1)
    let mapJson2 = JsonTraverser.traverseJsonDocument (json2)

    printfn "Json1 tartalma:"
    mapJson1 |> Map.iter (fun k v -> printfn "%s: %A" k v)

    printfn "\nJson2 tartalma:"
    mapJson2 |> Map.iter (fun k v -> printfn "%s: %A" k v)

    // Összehasonlítás és eredmény kiírása
    let comparisonResult = JsonComparator.compareJsonDictionaries mapJson1 mapJson2

    printfn "\nÖsszehasonlítás eredménye:"
    comparisonResult
    |> Map.iter (fun k v ->
        printfn "Kulcs: %s" k
        printfn "  Ugyanaz: %b" v.same
        printfn "  Json1 érték: %A" v.json1Value
        printfn "  Json2 érték: %A\n" v.json2Value
    )

    
    
    0 // Kilépési kód
