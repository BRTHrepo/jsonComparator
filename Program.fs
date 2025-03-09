module ConsoleApp1.Program
open System
open ConsoleApp1.Json
open ConsoleApp1.Json2




printfn "Hello from F#"

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
    let result = ConsoleApp1.Json.compareJsonFiles file1Path file2Path
    printfn $"Eredmény: %s{result}"
    
    let result2 = ConsoleApp1.Json2.compareJsonFiles file1Path file2Path
    result2 |> List.iter (printfn "%s")
    
    
    0 // Kilépési kód
