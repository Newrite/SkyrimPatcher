namespace Patcher

open Mutagen.Bethesda
open Mutagen.Bethesda.Skyrim
open Mutagen.Bethesda.Plugins.Cache
open Mutagen.Bethesda.FormKeys.SkyrimSE

[<RequireQualifiedAccess>]
module Armor =

  let private (|Heavy|Light|Clothing|EnumError|) (armorType: ArmorType) =
    match armorType with
    |ArmorType.HeavyArmor -> Heavy
    |ArmorType.LightArmor -> Light
    |ArmorType.Clothing -> Clothing
    |_ -> EnumError

  let private isShield (armor: #IArmorGetter) = 
    let intFlagShield = int(Armor.MajorFlag.Shield)
    let intFlagShieldAndNonPlayeble = int(Armor.MajorFlag.Shield) + int(Armor.MajorFlag.NonPlayable)
    let intArmorFlag = int(armor.MajorFlags)
    (intArmorFlag = intFlagShield) || (intArmorFlag = intFlagShieldAndNonPlayeble)

  [<RequireQualifiedAccess>]
  module private Templated =

    let armorsWithTemplate (armorCollect: #IArmorGetter seq) =

      printfn "Start get armors with template.\n"

      armorCollect
      |>Seq.filter ( fun armor -> 
        not armor.TemplateArmor.IsNull
      )

    let templatesWithChangedValues (cache: #ILinkCache) (armorCollect: #IArmorGetter seq) =

      printfn "Change templated armor gold value to gold value from template.\n"

      armorCollect
      |>Seq.map ( fun armor ->
        let fromTemplate =
            cache.ResolveAll<IArmorGetter>(armor.TemplateArmor.FormKey)
            |>Seq.map (fun x ->
              printfn "ID: %A Value: %d" x.EditorID x.Value
              x)
            |>Seq.head
        let copy = armor.DeepCopy()
        if  copy.Value <> fromTemplate.Value then
          printfn "Armor: %A from %A value: %d -> %d" copy.EditorID fromTemplate.EditorID copy.Value fromTemplate.Value
          copy.Value <- fromTemplate.Value
          copy, true
        else
          copy, false
      )

  [<RequireQualifiedAccess>]
  module private NoShield =

    let armorsWithoutTemplateNoShield (armorCollect: #IArmorGetter seq) =

      printfn "Start get armors (no shield) without template.\n"


      armorCollect
      |>Seq.filter ( fun armor ->
          
        armor.TemplateArmor.IsNull && not <| isShield armor
      )

    let armorsWithChangedDamageResist (armorCollect: #IArmorGetter seq) =

      printfn "Change armor (no shield) armor rating.\n"

      armorCollect
      |>Seq.map ( fun armor ->
        let copy = armor.DeepCopy()
        match copy.BodyTemplate.ArmorType with
        |Heavy ->
          if copy.ArmorRating > 0f then
            printfn "Armor: %A armor rating: %f -> %f" copy.EditorID copy.ArmorRating (armor.ArmorRating * 4.f)
            copy.ArmorRating <- armor.ArmorRating * 4.f
            copy, true
          else
            copy, false
        |Light ->
          if copy.ArmorRating > 0f then
            printfn "Armor: %A armor rating: %f -> %f" copy.EditorID copy.ArmorRating (armor.ArmorRating * 4.f)
            copy.ArmorRating <- armor.ArmorRating * 2.f
            copy, true
          else
            copy, false
        |Clothing ->
          printfn "Armor: %A is clothing" copy.EditorID
          copy, false
        |EnumError ->
          printfn "Error: incorrect Enum"
          copy, false
      )

  [<RequireQualifiedAccess>]
  module private Shield =

    let shieldWithoutTemplate (shieldCollect: #IArmorGetter seq) =

      printfn "Start get shield without template."


      shieldCollect
      |>Seq.filter ( fun shield ->
          
        shield.TemplateArmor.IsNull && isShield shield
      )

    let shieldWithChangedDamageResist (shieldCollect: #IArmorGetter seq) =

      printfn "Change shield armor rating.\n"

      shieldCollect
      |>Seq.map ( fun shield ->
        let copy = shield.DeepCopy()
        match copy.BodyTemplate.ArmorType with
        |Heavy ->
          printfn "Shield: %A armor rating: %f -> %f" copy.EditorID copy.ArmorRating (shield.ArmorRating * 4.f)
          copy.ArmorRating <- shield.ArmorRating * 4.f
          copy, true
        |Light ->
          printfn "Shield: %A armor rating: %f -> %f" copy.EditorID copy.ArmorRating (shield.ArmorRating * 4.f)
          copy.ArmorRating <- shield.ArmorRating * 2.f
          copy, true
        |Clothing ->
          printfn "Shield: %A is clothing" copy.EditorID
          copy, false
        |EnumError ->
          printfn "Error: incorrect Enum"
          copy, false
      )

    let addShieldKeywordToShields (cache: #ILinkCache) (shieldCollect: (Armor * bool) seq) =

      printfn "Add shield keyword for shield wihtout this keyword.\n"
      
      let shieldKeyword = Skyrim.Keyword.ArmorShield.Resolve<IKeywordGetter>(cache)

      shieldCollect
      |>Seq.map ( fun (shield, changed) ->
        match shield.HasKeyword(shieldKeyword), (isNull shield.Keywords) with
        |true, false ->
          printfn "Shield: %A already exist keyword" shield.EditorID
          shield, changed
        |false, false ->
          shield.Keywords.Add(shieldKeyword)
          printfn "Shield: %A add keyword" shield.EditorID
          shield, true
        |false, true ->
          shield.Keywords <- new Noggog.ExtendedList<_>()
          shield.Keywords.Add(shieldKeyword)
          printfn "Shield: %A add keyword" shield.EditorID
          shield, true
        |_ -> shield, changed
      )

  let private addArmorsRecordToMod (skyrimMod: #SkyrimMod) (armorCollect: (Armor * bool) seq) =
    
    printfn "Begin write armors or shields to mod...\n"

    armorCollect
    |>Seq.iter ( fun (armor, changed) ->
      if changed then
        printfn "Start write %A." armor.EditorID
        skyrimMod.Armors.GetOrAddAsOverride(armor)
        |>function
        |record when isNull record ->
          printfn "Faild add %A to mod, final record is null.\n" armor.EditorID
        |_ ->
          printfn "Success add %A to mod.\n" armor.EditorID
    )

  let processPatchArmorsAndShieldsWithTemplate
    (armorCollect: #IArmorGetter seq)
    (cache: #ILinkCache)
    (skyrimMod: #SkyrimMod) =

      armorCollect
      |> Templated.armorsWithTemplate
      |> Templated.templatesWithChangedValues cache
      |> addArmorsRecordToMod skyrimMod


  let processPatchArmorsWithoutTemplateNoShield
    (armorCollect: #IArmorGetter seq)
    (skyrimMod: #SkyrimMod) =

      armorCollect
      |> NoShield.armorsWithoutTemplateNoShield
      |> NoShield.armorsWithChangedDamageResist
      |> addArmorsRecordToMod skyrimMod

  let processPatchShieldsWithoutTemplate
    (armorCollect: #IArmorGetter seq)
    (cache: #ILinkCache)
    (skyrimMod: #SkyrimMod) =

      armorCollect
      |> Shield.shieldWithoutTemplate
      |> Shield.shieldWithChangedDamageResist
      |> Shield.addShieldKeywordToShields cache
      |> addArmorsRecordToMod skyrimMod