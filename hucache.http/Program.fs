open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config


[<EntryPoint>]
let main args =
    startWebServer defaultConfig (OK "Hello World!")
    0