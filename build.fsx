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

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
