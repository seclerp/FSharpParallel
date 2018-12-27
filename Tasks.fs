module Tasks

open System.IO
open System.Net
open Newtonsoft.Json
open System.Diagnostics
open System.Threading.Tasks
open System
open System.Collections.Concurrent
open System.Threading.Tasks
open FSharp.Collections.ParallelSeq

let request (url:string) =
  let req = HttpWebRequest.Create(url) :?> HttpWebRequest 
  let resp = req.GetResponse() 
  use stream = resp.GetResponseStream() 
  let reader = new StreamReader(stream) 
  reader.ReadToEnd()

[<CLIMutable>]
type CitiesResponse = {
  count : int
  list : array<City>
}
and City = {
  name : string
  id : int
}

[<CLIMutable>]
type WeatherResponse = {
  main : Weather
}
and Weather = {
  temp : float
  pressure : float
}

let getCities apiKey =
  request <| sprintf "https://api.openweathermap.org/data/2.5/find?lat=49&lon=32&cnt=50&appid=%s" apiKey   // 49, 32 - coordinates of arbitary center of Ukraine
  |> JsonConvert.DeserializeObject<CitiesResponse>

let getInfo apiKey id  =
  request <| sprintf "https://api.openweathermap.org/data/2.5/weather?id=%i&appid=%s" id apiKey
  |> JsonConvert.DeserializeObject<WeatherResponse>

let getAverageInfo apiKey citiesResponse  =
  let weathersInfo = citiesResponse.list |> Seq.map (fun city -> getInfo apiKey city.id)
  let averageTemp = weathersInfo |> Seq.averageBy (fun weather -> weather.main.temp)
  let averagePressure = weathersInfo |> Seq.averageBy (fun weather -> weather.main.pressure)
  { temp = averageTemp; pressure = averagePressure }

let getAverageInfoArrayParallel apiKey citiesResponse =
  let weatherTasks = citiesResponse.list |> Array.Parallel.map (fun city -> getInfo apiKey (city.id))
  let averageTemp = weatherTasks |> Array.averageBy (fun weather -> weather.main.temp)
  let averagePressure = weatherTasks |> Seq.averageBy (fun weather -> weather.main.pressure)
  { temp = averageTemp; pressure = averagePressure }

let getAverageInfoPSeq apiKey citiesResponse =
  let weatherTasks = citiesResponse.list |> PSeq.map (fun city -> getInfo apiKey (city.id))
  let averageTemp = weatherTasks |> PSeq.averageBy (fun weather -> weather.main.temp)
  let averagePressure = weatherTasks |> PSeq.averageBy (fun weather -> weather.main.pressure)
  { temp = averageTemp; pressure = averagePressure }

let executeExample apiKey =
  let watch = Stopwatch()
  watch.Start()
  let averageInfo = apiKey |> getCities |> getAverageInfo apiKey
  watch.Stop()
  printfn "Average weather info, synchronous:    Temperature: %fC, Pressure: %f, done in %A" 
    (averageInfo.temp - 273.) averageInfo.pressure watch.Elapsed

  let watch = Stopwatch()
  watch.Start()
  let averageInfo = apiKey |> getCities |> getAverageInfoArrayParallel apiKey
  watch.Stop()
  printfn "Average weather info, Array.Parallel: Temperature: %fC, Pressure: %f, done in %A" 
    (averageInfo.temp - 273.) averageInfo.pressure watch.Elapsed

  let watch = Stopwatch()
  watch.Start()
  let averageInfo = apiKey |> getCities |> getAverageInfoPSeq apiKey
  watch.Stop()
  printfn "Average weather info, PSeq:           Temperature: %fC, Pressure: %f, done in %A" 
    (averageInfo.temp - 273.) averageInfo.pressure watch.Elapsed