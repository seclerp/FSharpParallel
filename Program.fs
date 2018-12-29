[<EntryPoint>]
let main argv =
  // Threads
  printfn "Threads example:"
  Threads.executeExample 10
  Threads.executeExample 1000
  Threads.executeExample 10000
  Threads.executeExample 100000

  // Thread pools
  printfn "\nThread pools example:"
  ThreadPools.executeExample 10
  ThreadPools.executeExample 1000
  ThreadPools.executeExample 10000
  ThreadPools.executeExample 100000

  // Tasks
  printfn "\nTasks example:"
  Tasks.executeExample "7751e64db1ace366d92d3ca6d7b0e1ed"

  // Async workflow
  printfn "\nAsync workflow example:"
  Async.executeExample ()
  0 // exit code