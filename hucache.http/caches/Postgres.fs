module hucache.http.caches.postgres


open Npgsql;
open hucache.http.types
open System.Text.RegularExpressions
open Dapper
open System.Dynamic
open System.Linq
open System.Collections.Generic

let convertHerokuUrl (input : string ) : string = 
    let urb = new System.UriBuilder(input)
    let db = urb.Path.Replace("/","")
    let cs = (sprintf "User ID=%s;Password=%s;Host=%s;Port=%i;Database=%s;Pooling=false;" urb.UserName urb.Password urb.Host urb.Port db)
    cs
    
let connectionString = 
    let raw = System.Environment.GetEnvironmentVariable("DATABASE_URL")
    convertHerokuUrl raw

//Should turn a map into a PGSQL HSTORE 'a=>1,b=>2'::hstore
let serialize (headers:Map<string,string>) : Dictionary<string, string> =
    let dict = new Dictionary<string, string>()
    headers 
    |> Map.iter (fun k v ->
                 let vv = v.Replace("W/","").Replace("\"","")
                 dict.Add(k, vv))
    dict

let deserialize (raw : IDictionary<string, string>) : Map<string, string> =
    match raw with 
    | null -> Map.empty
    | r -> (r :> seq<_>)
           |> Seq.map (|KeyValue|)
           |> Map.ofSeq
    

type R(headers : IDictionary<string, string>, payload : string) =
    new() = R(new Dictionary<string, string>(), "")
    member val Headers = headers with get, set
    member val Payload = payload with get, set

let loadIssue (key:IssueKey) : FullPayload option = 
    use conn = new Npgsql.NpgsqlConnection(connectionString)

    let p = ExpandoObject() 
    let pp = p :> IDictionary<string, obj>
    pp.Add("owner", key.owner)
    pp.Add("repo", key.repo)
    pp.Add("issue", key.issue)
              
    conn.Query<R>("SELECT headers AS Headers, payload AS Payload 
                   FROM github.issues
                   WHERE owner = @owner
                   AND repo = @repo
                   AND issue = @issue", p)
    |> Seq.tryPick (fun r ->
            let h = deserialize r.Headers
            let p = r.Payload
            Some {headers=h; payload=p}
            )


let storeIssue (key:IssueKey, payload:FullPayload) : FullPayload = 
  use conn = new Npgsql.NpgsqlConnection(connectionString)
  let h = serialize payload.headers
  conn.Open() 
  use cmd = conn.CreateCommand()
  cmd.CommandText <- "INSERT INTO github.issues (owner, repo, issue, headers, payload)
                    VALUES (@owner, @repo, @issue, @headers::hstore, @payload::jsonb)"
  cmd.Parameters.Add("owner", NpgsqlTypes.NpgsqlDbType.Text).Value <- key.owner
  cmd.Parameters.Add("repo", NpgsqlTypes.NpgsqlDbType.Text).Value <- key.repo
  cmd.Parameters.Add("issue", NpgsqlTypes.NpgsqlDbType.Integer).Value <- key.issue
  cmd.Parameters.Add("headers", NpgsqlTypes.NpgsqlDbType.Hstore).Value <- payload.headers
  cmd.Parameters.Add("payload", NpgsqlTypes.NpgsqlDbType.Jsonb).Value <- payload.payload
  
  
  cmd.ExecuteNonQuery() |> ignore

  payload