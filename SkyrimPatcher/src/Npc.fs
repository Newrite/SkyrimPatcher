namespace Patcher

open Mutagen.Bethesda
open Mutagen.Bethesda.Skyrim
open Mutagen.Func
  
[<RequireQualifiedAccess>]
module Npc =

  let private templateScriptEntryNpc staticAmount multAmount =
    Scripts.ScriptEntry.create "XP" [
      Scripts.ScriptProperty.float "XPStaticAmount" staticAmount
      Scripts.ScriptProperty.float "XPMultAmount" multAmount
    ]

  let private filterWithoutScriptTemplateFlag (npcCollection: #INpcGetter seq) =
    
    npcCollection
    |> Seq.filter ( fun npc ->
      let x = npc.Configuration.TemplateFlags
      not <| x.HasFlag(NpcConfiguration.TemplateFlag.Script)
    )

  let private attachXPScript (npcCollection: #INpcGetter seq) =
    
    printfn "Start attach script to npcs.\n"

    npcCollection
    |> Seq.map ( fun npc ->
      let copy = npc.DeepCopy()
      if isNull copy.VirtualMachineAdapter then
        let newVMA =
          Scripts.VirtualMachineAdapter.create [ templateScriptEntryNpc 10000.f 2.f ]
        copy.VirtualMachineAdapter <- newVMA
        printfn "NPC: %A null VMA, create new and add script" copy.EditorID
        copy
      else
        let newScriptEntry = templateScriptEntryNpc 10000.f 2.f
        copy.VirtualMachineAdapter.Scripts.Add(newScriptEntry)
        printfn "NPC: %A add script" copy.EditorID
        copy
    )

  let private addNpcRecordToMod (skyrimMod: #SkyrimMod) (npcCollection: Npc seq) =
    
    printfn "Begin write npc to mod...\n"

    npcCollection
    |>Seq.iter ( fun npc ->
      printfn "Start write %A." npc.EditorID
      skyrimMod.Npcs.GetOrAddAsOverride(npc)
      |>function
      |record when isNull record ->
        printfn "Faild add %A to mod, final record is null.\n" npc.EditorID
      |_ ->
        printfn "Success add %A to mod.\n" npc.EditorID
    )

  let processPatchNpc
    (npcCollection: #INpcGetter seq)
    (skyrimMod: #SkyrimMod) =

      npcCollection
      |> filterWithoutScriptTemplateFlag
      |> attachXPScript
      |> addNpcRecordToMod skyrimMod