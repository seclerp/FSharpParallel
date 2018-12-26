module Threads

open System.Threading

let startHelloWorldThread num =
    let work () = 
        for i in [1..10] do
            printfn "Hello from %i thread! Iteration %i" num i
            Thread.Sleep(1000)
    let newThread = Thread(work)
    newThread.Start()

let executeExample () =
    [1..10] |> Seq.iter startHelloWorldThread