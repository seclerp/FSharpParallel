module Async

open System.Net
open System
open System.IO
open System.Diagnostics

let fetchHtml url =
  let req = WebRequest.Create(Uri(url)) 
  use resp = req.GetResponse() 
  use stream = resp.GetResponseStream() 
  use reader = new StreamReader(stream) 
  reader.ReadToEnd()

let asyncFetchHtml url =
  async {
    let req = WebRequest.Create(Uri(url)) 
    use! resp = req.AsyncGetResponse() 
    use stream = resp.GetResponseStream() 
    use reader = new StreamReader(stream) 
    return reader.ReadToEnd()
  }

let executeExample () =
  let exampleUrls = [
    "http://www.polygon.com/"; "http://www.theverge.com/"; "http://nure.ua"; 
    "http://www.condenast.com/"; "http://theblackharbor.com/"; "http://www.usatoday.com/"; 
    "http://www.newyorker.com/"; "http://www.aiga.org/"; "http://www.alistapart.com/";
    "http://www.ebony.com/"; "http://www.wired.com/"; "http://bostonglobe.com";
  ]

  let watch = Stopwatch()
  watch.Start()
  exampleUrls |> List.map fetchHtml |> ignore
  watch.Stop()
  printfn "Fetched %i URLs, synchronous,    done in %A" (List.length exampleUrls) watch.Elapsed

  let watch = Stopwatch()
  watch.Start()
  exampleUrls |> List.map asyncFetchHtml |> Async.Parallel |> Async.RunSynchronously |> ignore
  watch.Stop()
  printfn "Fetched %i URLs, async workflow, done in %A" (List.length exampleUrls) watch.Elapsed