namespace Patcher

open Mutagen.Bethesda
open Mutagen.Bethesda.Skyrim
open Mutagen.Bethesda.Plugins.Cache
open Mutagen.Bethesda.FormKeys.SkyrimSE
  
[<RequireQualifiedAccess>]
module LeveledNpc =

  let private deepCopyLeveledNpcs (leveledNpcCollection: #ILeveledNpcGetter seq) =
    
    printfn "Create deep copy of all leveled npc...\n"

    leveledNpcCollection
    |>Seq.map ( fun leveledNpc ->
      leveledNpc.DeepCopy()  
    )

  let private changeLevelEntries (leveledNpcCollection: #LeveledNpc seq) =

    printfn "Start change level for leveled npc entries.\n"

    let calcLevel level =
      match level with
      |l when l < 15s -> 1s
      |l when l >= 15s && l < 30s -> 15s
      |_ -> 30s


    leveledNpcCollection
    |> Seq.map ( fun leveledNpc ->
      leveledNpc.Entries
      |> Seq.iter ( fun npc ->
        let newLevel = calcLevel npc.Data.Level
        printfn "Change level from %d to %d..." npc.Data.Level newLevel
        npc.Data.Level <- newLevel
      )
      leveledNpc 
    )

  let private addLeveledNpcRecordToMod (skyrimMod: #SkyrimMod) (leveledNpcCollection: LeveledNpc seq) =
    
    printfn "Begin write LeveledNpc or shields to mod...\n"

    leveledNpcCollection
    |>Seq.iter ( fun leveledNpc ->
      printfn "Start write %A." leveledNpc.EditorID
      skyrimMod.LeveledNpcs.GetOrAddAsOverride(leveledNpc)
      |>function
      |record when isNull record ->
        printfn "Faild add %A to mod, final record is null.\n" leveledNpc.EditorID
      |_ ->
        printfn "Success add %A to mod.\n" leveledNpc.EditorID
    )

  let processPatchLeveledNpc
    (leveledNpcCollection: #ILeveledNpcGetter seq)
    (skyrimMod: #SkyrimMod) =

      leveledNpcCollection
      |> deepCopyLeveledNpcs
      |> changeLevelEntries
      |> addLeveledNpcRecordToMod skyrimMod