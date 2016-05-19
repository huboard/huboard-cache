// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.XUnit2

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
      ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (buildDir @@ "*.tests.dll")
    |> xUnit2 (fun p -> { p with 
                            HtmlOutputPath = Some ("xunit.html");
                            ToolPath = "packages/xunit.runner.console/tools/xunit.console.exe";
    })
    |> ignore
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

Target "Migrate" (fun _ ->
    Roundhouse (fun p -> { p with
        SqlFilesDirectory = "./migrations"
        ServerDatabase = "localhost"
        DatabaseName = "hucache"
        WarnOnOneTimeScriptChanges = true
	})

Target "Watch" (fun _ ->
    use watcher = (!! "build/*.dll") |> WatchChanges (fun changes -> 
        tracefn "%A" changes
        Run "Test")
    
    System.Console.ReadLine() |> ignore //Needed to keep FAKE from exiting

    watcher.Dispose() // Use to stop the watch from elsewhere, ie another task.
)

Target "Docker" (fun _ ->
    ExecProcess (fun info ->
        info.FileName <- "docker"
        info.Arguments <- "build -t cache ."
        info.WorkingDirectory <- ".") (System.TimeSpan.FromMinutes 5.0) |> ignore
)
// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
