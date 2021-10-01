open Mutagen.Func
open Mutagen.Bethesda.Skyrim

[<EntryPoint>]
let main _ =

  let loadOrder = LoadOrder.PriorityOrderFromEnv SkyrimRelease.SkyrimLE
  let cache = Cache.ToImmutableLinkCache loadOrder
  let armorsRecord =
    Records.Armor.WinningOverrides false (Seq.rev loadOrder)
    |>Seq.filter (isNull >> not)

  let modName = "ArmorTest.esp"

  System.IO.File.Delete(modName)

  let newMod = Mods.CreateSkyrimMod modName SkyrimRelease.SkyrimLE

  Patcher.Armor.processPatchArmorsAndShieldsWithTemplate armorsRecord cache newMod
  Patcher.Armor.processPatchArmorsWithoutTemplateNoShield armorsRecord newMod
  Patcher.Armor.processPatchShieldsWithoutTemplate armorsRecord cache newMod

  newMod.WriteToBinaryParallel(modName)

  printfn "Patch complete. Any key to exit."
  System.Console.ReadKey() |> ignore

  0
