module hucache.http.github

open FSharp.Data
open hucache.http.types

let fetchIssue (issue: IssueKey) : FullPayload option =
  let url = (sprintf "https://api.github.com/repos/%s/%s/issues/%d" issue.owner issue.repo issue.issue)  
  let req = Http.Request(url, headers=["User-Agent","HuCache"])
  //need a match in here some place for 404
  let headers = req.Headers
  let body = match req.Body with
             | Text text -> text
             | Binary _ -> "{}"
  Some {headers=headers; payload=body}