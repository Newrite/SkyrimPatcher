open Mutagen.Func
open Mutagen.Bethesda.Skyrim

[<EntryPoint>]
let main _ =

  let loadOrder = LoadOrder.PriorityOrderFromEnv SkyrimRelease.SkyrimLE
  let cache = Cache.ToImmutableLinkCache loadOrder

  let modName = "ArmorTest.esp"
  System.IO.File.Delete(modName)
  let newMod = Mods.CreateSkyrimMod modName SkyrimRelease.SkyrimLE

  let armorsRecord =
    Records.Armor.WinningOverrides false (Seq.rev loadOrder)
    |>Seq.filter (isNull >> not)

  let leveledNpcsRecord =
    Records.LeveledNpc.WinningOverrides false (Seq.rev loadOrder)
    |> Seq.filter (isNull >> not)
    |> Seq.filter (fun leveledNpc -> not <| isNull leveledNpc.Entries)

  let npcsRedord =
    Records.Npc.WinningOverrides false (Seq.rev loadOrder)
    |>Seq.filter (isNull >> not)

  Patcher.Armor.processPatchArmorsAndShieldsWithTemplate armorsRecord cache newMod
  Patcher.Armor.processPatchArmorsWithoutTemplateNoShield armorsRecord newMod
  Patcher.Armor.processPatchShieldsWithoutTemplate armorsRecord cache newMod

  Patcher.LeveledNpc.processPatchLeveledNpc leveledNpcsRecord newMod

  Patcher.Npc.processPatchNpc npcsRedord newMod

  newMod.WriteToBinaryParallel(modName)

  printfn "Patch complete. Any key to exit."
  System.Console.ReadKey() |> ignore

  0
