module hucache.http.caches.postgres


open Npgsql;
open hucache.http.types
open System.Text.RegularExpressions
open Dapper
open System.Dynamic
open System.Linq
open System.Collections.Generic


let connectionString = System.Environment.GetEnvironmentVariable("PG") 


//Should turn a map into a PGSQL HSTORE 'a=>1,b=>2'::hstore
let serialize (headers:Map<string,string>) : string = 
    headers
    |> Seq.map (fun e ->
        let vv = e.Value.Replace("W/","").Replace("\"","")
        let v = (sprintf "\"%s\"=>\"%s\"" e.Key vv)
        v)
    |> String.concat ","
    
let s(arr:string[]) (i:int32): string = 
    if((arr.Length - 1)>=i) then
        arr.[i]
    else
        ""
    
//parses a "a=>1,b=>" into a Map<string, string>
let deserialize (raw : string) : Map<string, string> =
    match raw with 
    | null -> Map.empty
    | r -> r.Trim().Split [|','|]
                |> Seq.map (fun h -> Regex.Split(h, "=>"))
                |> Seq.map (fun a -> ((s a 0), (s a 1)))
                |> Map.ofSeq
    

type R(headers : string, payload : string) =
    new() = R("","")
    member val Headers = headers with get, set
    member val Payload = payload with get, set

let getFromDb (key:IssueKey) : FullPayload option = 
    use conn = new Npgsql.NpgsqlConnection(connectionString)

    let p = ExpandoObject() 
    let pp = p :> IDictionary<string, obj>
    pp.Add("owner", key.owner)
    pp.Add("repo", key.repo)
    pp.Add("issue", key.issue)
              
    conn.Query<R>("SELECT headers::text AS Headers, payload AS Payload 
                   FROM public.issues
                   WHERE owner = @owner
                   AND repo = @repo
                   AND issue = @issue", p)
    |> Seq.tryPick (fun r ->
            let h = deserialize r.Headers
            let p = r.Payload
            Some {headers=h; payload=p}
            )

let storeInDb (key:IssueKey, payload:FullPayload) : FullPayload = 
  use conn = new Npgsql.NpgsqlConnection(connectionString)
  let h = serialize payload.headers

  let p = ExpandoObject() 
  let pp = p :> IDictionary<string, obj>
  pp.Add("owner", key.owner)
  pp.Add("repo", key.repo)
  pp.Add("issue", key.issue)
  pp.Add("headers", h)
  pp.Add("payload", payload.payload :> obj)
  ignore <| conn.Execute("INSERT INTO public.issues (owner, repo, issue, headers, payload)
                          VALUES (@owner, @repo, @issue, @headers::hstore, @payload::json)",p)
  payload