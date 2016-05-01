namespace hucache.http

open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Filters
open Suave.Operators
open System.Net
open hucache.http.actions



module Program =
  let app =
    choose
      [ GET >=> choose
          [ pathScan "/cache/%s/%s/issues/%d" getIssue
            pathScan "/cache/%s/%s/issues" getIssues ]]

  let port =
      let couldParse, parsedInt = System.UInt16.TryParse(System.Environment.GetEnvironmentVariable("PORT"))
      if couldParse then parsedInt
      else 5000us
      
  let config =
    let ip = IPAddress.Parse "0.0.0.0"
    {defaultConfig with
      bindings = [HttpBinding.mk HTTP ip port ] }

  [<EntryPoint>]
  let main argv =
    startWebServer config app
    0
