open Mutagen.Func
open Mutagen.Bethesda.Skyrim

[<EntryPoint>]
let main _ =

  let loadOrder =
    LoadOrder.skyrimPriorityOrderFromEnv SkyrimRelease.SkyrimLE
    |> Seq.filter ( fun aMod -> aMod.Enabled )
  let cache = Cache.toImmutableLinkCache loadOrder

  let modName = "ArmorTest.esp"
  System.IO.File.Delete(modName)
  let newMod = Mods.createSkyrimMod modName SkyrimRelease.SkyrimLE

  let armorsRecord =
    Records.Skyrim.Armor.winningOverrides false (Seq.rev loadOrder)
    |>Seq.filter (isNull >> not)

  loadOrder
  |> Seq.iter ( fun x ->
    x.Mod.Keywords.FormKeys
    |> Seq.iter ( fun y ->
      x.Mod.Keywords.RecordCache
      |> Seq.iter ( fun qwe -> printfn "%A" qwe.Value.EditorID)
      printfn "Keyword ID %0x %s" y.ID (let z = y.ModKey in z.Name)
    )
    let name = let a = x.ModKey in a.FileName
    name.String |> printfn "Mod name: %s"
  )

  let leveledNpcsRecord =
    Records.Skyrim.LeveledNpc.winningOverrides false (Seq.rev loadOrder)
    |> Seq.filter (isNull >> not)
    |> Seq.filter (fun leveledNpc -> not <| isNull leveledNpc.Entries)

  let npcsRedord =
    Records.Skyrim.Npc.winningOverrides false (Seq.rev loadOrder)
    |>Seq.filter (isNull >> not)

  //Patcher.Armor.processPatchArmorsAndShieldsWithTemplate armorsRecord cache newMod
  //Patcher.Armor.processPatchArmorsWithoutTemplateNoShield armorsRecord newMod
  //Patcher.Armor.processPatchShieldsWithoutTemplate armorsRecord cache newMod
  //
  //Patcher.LeveledNpc.processPatchLeveledNpc leveledNpcsRecord newMod
  //
  //Patcher.Npc.processPatchNpc npcsRedord newMod
  //
  //newMod.WriteToBinaryParallel(modName)

  printfn "Patch complete. Any key to exit."
  System.Console.ReadKey() |> ignore

  0
