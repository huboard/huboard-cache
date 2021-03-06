module hucache.http.types

open FSharp.Data

type RepoKey = {owner:string; repo:string;}
type IssueKey = {owner:string; repo:string; issue:int;}
type Issue = JsonProvider<"./data/Issue.json">
type FullPayload = {headers: Map<string, string>; payload:string;}
