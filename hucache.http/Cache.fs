module hucache.http.cache

open hucache.http.caches.postgres
open hucache.http.types
open hucache.http.github

let lookup (key: IssueKey) : FullPayload option =
    match loadIssue key with
    | Some payload -> Some(payload)
    | None -> 
        let issue = fetchIssue key
        match issue with 
        | Some i -> Some((storeIssue (key, i)))
        | None -> None
