namespace hucache.http

open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Filters
open Suave.Operators
open hucache.http.actions



module Program = 
  let app = 
    choose 
      [ GET >=> choose
          [ pathScan "/cache/%s/%s/issues/%d" getIssue
            pathScan "/cache/%s/%s/issues" getIssues ]]

  [<EntryPoint>]
  let main argv =           
    startWebServer defaultConfig app
    0
