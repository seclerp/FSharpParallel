module ThreadPools

open System.Threading
open System.IO
open System.Diagnostics

let writeFile num () = 
  File.WriteAllText("Files/" + string num + ".txt", "Hello from F#, file #" + string num)

let executeExample filesCount =
  printfn "Start processing of %i files" filesCount
  Directory.CreateDirectory("Files") |> ignore
  let watch1 = Stopwatch()
  watch1.Start()
  for i in [1..filesCount] do writeFile i ()
  watch1.Stop()
  printfn "Write files task, synchronous\t\t\t%A" watch1.Elapsed

  let watch2 = Stopwatch()
  watch2.Start()
  use countdownEvent = new CountdownEvent(filesCount)
  for i in [1..filesCount] do 
    ThreadPool.QueueUserWorkItem(
      new WaitCallback(fun _ -> writeFile i (); countdownEvent.Signal() |> ignore)
    ) |> ignore
  countdownEvent.Wait()
  watch2.Stop()
  printfn "Write files task, thread from threadpool:\t%A" watch2.Elapsed