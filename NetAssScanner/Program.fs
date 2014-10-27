// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

let GetContent (fname:string) =
    let assDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(fname)
    assDef.Name.ToString()
    

[<EntryPoint>]
let main argv = 
    let files = System.IO.DirectoryInfo(@"c:\Development\dev3\MultiVox\References\Debug").GetFiles("*.dll", System.IO.SearchOption.AllDirectories)
    

    let printerAgent = MailboxProcessor.Start(fun inbox-> 
        let rec messageLoop = async{
            let! msg = inbox.Receive()
            printfn "message is: %s" msg
            return! messageLoop  
            }
        messageLoop 
    )

    
    files        
        |> Array.map(fun f -> GetContent(f.FullName))
        |> Array.map(fun msg -> printfn "message is: %s" msg)
        |> ignore

//    [1..100000]
//        |> List.map(fun x -> printerAgent.Post(System.String.Format("Cool: {0}", x)))
//        |> ignore

    //System.Console.ReadKey() |> ignore
    0 // return an integer exit code
