



namespace Patcher
    
    module Npc =
        
        val private templateScriptEntryNpc:
          staticAmount: float32 -> multAmount: float32
            -> Mutagen.Bethesda.Skyrim.ScriptEntry
        
        val private filterWithoutScriptTemplateFlag:
          npcCollection: seq<'a> -> seq<'a>
            when 'a :> Mutagen.Bethesda.Skyrim.INpcGetter
        
        val private attachXPScript:
          npcCollection: seq<#Mutagen.Bethesda.Skyrim.INpcGetter>
            -> seq<Mutagen.Bethesda.Skyrim.Npc>
        
        val private addNpcRecordToMod:
          skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod
          -> npcCollection: seq<Mutagen.Bethesda.Skyrim.Npc> -> unit
        
        val processPatchNpc:
          npcCollection: seq<#Mutagen.Bethesda.Skyrim.INpcGetter>
          -> skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod -> unit

namespace Patcher
    
    module LeveledNpc =
        
        val private deepCopyLeveledNpcs:
          leveledNpcCollection: seq<#Mutagen.Bethesda.Skyrim.ILeveledNpcGetter>
            -> seq<Mutagen.Bethesda.Skyrim.LeveledNpc>
        
        val private changeLevelEntries:
          leveledNpcCollection: seq<'a> -> seq<'a>
            when 'a :> Mutagen.Bethesda.Skyrim.LeveledNpc
        
        val private addLeveledNpcRecordToMod:
          skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod
          -> leveledNpcCollection: seq<Mutagen.Bethesda.Skyrim.LeveledNpc>
            -> unit
        
        val processPatchLeveledNpc:
          leveledNpcCollection: seq<#Mutagen.Bethesda.Skyrim.ILeveledNpcGetter>
          -> skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod -> unit

namespace Patcher
    
    module Armor =
        
        val private (|Heavy|Light|Clothing|EnumError|) :
          armorType: Mutagen.Bethesda.Skyrim.ArmorType
            -> Choice<unit,unit,unit,unit>
        
        val private isShield:
          armor: #Mutagen.Bethesda.Skyrim.IArmorGetter -> bool
        
        module private Templated =
            
            val armorsWithTemplate:
              armorCollect: seq<'a> -> seq<'a>
                when 'a :> Mutagen.Bethesda.Skyrim.IArmorGetter
            
            val templatesWithChangedValues:
              cache: #Mutagen.Bethesda.Plugins.Cache.ILinkCache
              -> armorCollect: seq<#Mutagen.Bethesda.Skyrim.IArmorGetter>
                -> seq<Mutagen.Bethesda.Skyrim.Armor * bool>
        
        module private NoShield =
            
            val armorsWithoutTemplateNoShield:
              armorCollect: seq<'a> -> seq<'a>
                when 'a :> Mutagen.Bethesda.Skyrim.IArmorGetter
            
            val armorsWithChangedDamageResist:
              armorCollect: seq<#Mutagen.Bethesda.Skyrim.IArmorGetter>
                -> seq<Mutagen.Bethesda.Skyrim.Armor * bool>
        
        module private Shield =
            
            val shieldWithoutTemplate:
              shieldCollect: seq<'a> -> seq<'a>
                when 'a :> Mutagen.Bethesda.Skyrim.IArmorGetter
            
            val shieldWithChangedDamageResist:
              shieldCollect: seq<#Mutagen.Bethesda.Skyrim.IArmorGetter>
                -> seq<Mutagen.Bethesda.Skyrim.Armor * bool>
            
            val addShieldKeywordToShields:
              cache: #Mutagen.Bethesda.Plugins.Cache.ILinkCache
              -> shieldCollect: seq<Mutagen.Bethesda.Skyrim.Armor * bool>
                -> seq<Mutagen.Bethesda.Skyrim.Armor * bool>
        
        val private addArmorsRecordToMod:
          skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod
          -> armorCollect: seq<Mutagen.Bethesda.Skyrim.Armor * bool> -> unit
        
        val processPatchArmorsAndShieldsWithTemplate:
          armorCollect: seq<#Mutagen.Bethesda.Skyrim.IArmorGetter>
          -> cache: #Mutagen.Bethesda.Plugins.Cache.ILinkCache
          -> skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod -> unit
        
        val processPatchArmorsWithoutTemplateNoShield:
          armorCollect: seq<#Mutagen.Bethesda.Skyrim.IArmorGetter>
          -> skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod -> unit
        
        val processPatchShieldsWithoutTemplate:
          armorCollect: seq<#Mutagen.Bethesda.Skyrim.IArmorGetter>
          -> cache: #Mutagen.Bethesda.Plugins.Cache.ILinkCache
          -> skyrimMod: #Mutagen.Bethesda.Skyrim.SkyrimMod -> unit


module Main

val main: string[] -> int

