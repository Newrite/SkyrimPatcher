#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget FSharp.Core 5.0.0 //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.DotNet

let buildDir = "./build/"
let skyrimDir = @"G:\MO2Dev\overwrite"

Target.create "Clean" (fun _ ->
  Shell.cleanDir buildDir
)

Target.create "PublishPatcher" (fun _ ->

  let setPublishParamExe (defaults: DotNet.PublishOptions) =
    { defaults with
        Runtime = Some "win-x64"
        OutputPath = Some buildDir
        SelfContained = Some true
        MSBuildParams =
        { MSBuild.CliArguments.Create() with
            Properties =
            [
              "Optimize", "True"
              "PublishSingleFile", "True"
            ]
        }
    }
  
  DotNet.publish setPublishParamExe "./SkyrimPatcher/SkyrimPatcher.fsproj"
)

Target.create "MovePatcher" (fun _ ->
  Shell.copyFile skyrimDir (buildDir + "SkyrimPatcher.exe")
)

open Fake.Core.TargetOperators

"Clean"
  ==> "PublishPatcher"
  ==> "MovePatcher"

Target.runOrDefault "MovePatcher"