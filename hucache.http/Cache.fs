module hucache.http.cache

open hucache.http.caches.postgres
open hucache.http.types
open hucache.http.github

let lookupIssue (key: IssueKey) : FullPayload option =
    match loadIssue key with
    | Some payload -> Some(payload)
    | None -> 
        let issue = fetchIssue key
        match issue with 
        | Some i -> Some((storeIssue (key, i)))
        | None -> None

let lookupIssues (key: RepoKey) : FullPayload list = 
    match loadIssues key with 
    | [] -> []
    | i -> i 