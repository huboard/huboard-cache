module hucache.http.actions

open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Filters
open Suave.Writers
open Suave.Operators
open hucache.http.cache

let getIssue (owner : string, repo : string, issue : int32) : WebPart =
  let issue = lookupIssue {owner=owner; repo=repo; issue= issue}
  match issue with
  | Some i -> OK i.payload >=> setMimeType "application/json" //need to tweak headers
  | None -> RequestErrors.NOT_FOUND "Found no issues"

let getIssues (owner, repo) : WebPart =
  let issues = lookupIssues {owner=owner; repo=repo;}
  let payload = issues |> (Seq.map (fun i -> i.payload)) |> String.concat ","
  
  
  match issues with 
  | [] -> RequestErrors.NOT_FOUND "Found no issues"
  | _ -> (OK ("[" + payload + "]")) >=> setMimeType "application/json"
 
 