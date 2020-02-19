using Verse;
using Harmony;
using RimWorld;
using System;

namespace LootingManager
{
    [StaticConstructorOnStartup]
    static class HarmonyPatch
    {
        static HarmonyPatch()
        {
            var harmony = HarmonyInstance.Create("rimworld.carnysenpai.lootingmanager");
            harmony.Patch(AccessTools.Method(typeof(Pawn), "Kill"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Kill_PostFix")), null);
            harmony.Patch(AccessTools.Method(typeof(ThingOwner), "TryDrop", new[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(ThingPlaceMode), typeof(int), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>), typeof(Predicate<IntVec3>) }), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryDrop_PostFix")), null);
            harmony.Patch(AccessTools.Method(typeof(ThingOwner), "TryDrop", new[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>), typeof(Predicate<IntVec3>) }), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryDrop_PostFix")), null);
        }

        [HarmonyPostfix]
        public static void Kill_PostFix(Pawn __instance)
        {
            if (__instance.Dead && __instance.RaceProps.Humanlike && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteCorpses)
            {
                if (__instance.Faction == Faction.OfPlayer) return;
                if (__instance.IsPrisonerOfColony && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().excludePrisoners) return;
                if (__instance.Faction.HostileTo(Faction.OfPlayer) && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteHostile) return;
                if (!__instance.Faction.HostileTo(Faction.OfPlayer) && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteFriendly) return;
                __instance.inventory.DropAllNearPawn(__instance.PositionHeld);
                __instance.apparel.DropAll(__instance.PositionHeld);
                __instance.equipment.DropAllEquipment(__instance.PositionHeld);
                __instance.Corpse.Destroy(DestroyMode.Vanish);
            }
        }

        [HarmonyPostfix]
        public static void TryDrop_PostFix(bool __result, ThingOwner __instance, Thing thing)
        {
            if (!__result) return;

            Pawn holdingPawn = null;
            if (holdingPawn == null && __instance.Owner is Pawn_EquipmentTracker pawn_EquipmentTracker) holdingPawn = pawn_EquipmentTracker.pawn;
            if (holdingPawn == null && __instance.Owner is Pawn_InventoryTracker pawn_InventoryTracker) holdingPawn = pawn_InventoryTracker.pawn;
            if (holdingPawn == null && __instance.Owner is Pawn_ApparelTracker pawn_ApparelTracker) holdingPawn = pawn_ApparelTracker.pawn;
            if (holdingPawn == null) return;

            if (holdingPawn.Faction == Faction.OfPlayer) return;
            if (holdingPawn.IsPrisonerOfColony && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().excludePrisoners) return;
            if (holdingPawn.Faction.HostileTo(Faction.OfPlayer) && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteHostile) return;
            if (!holdingPawn.Faction.HostileTo(Faction.OfPlayer) && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteFriendly) return;

            if (thing.def.IsWeapon && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteWeapons) return;
            if (thing.def.IsApparel && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteApparel) return;
            if (!thing.def.IsWeapon && !thing.def.IsApparel && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteEverythingElse) return;

            if (LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteOnlyUnresearched && thing.def.IsResearchFinished && (thing.def.recipeMaker == null || thing.def.recipeMaker.researchPrerequisite == null || thing.def.recipeMaker.researchPrerequisite.IsFinished)) return;

            if (!Rand.Chance(LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteChance)) return;

            if (LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().refundItems)
            {
                foreach (Thing product in thing.SmeltProducts(LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().refundEfficiency))
                {
                    ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(product.def);
                    thingWithComps.stackCount = product.stackCount;
                    GenSpawn.Spawn(thingWithComps, holdingPawn.PositionHeld, holdingPawn.MapHeld);
                }
            }
            thing.Destroy(DestroyMode.Vanish);
        }
    }
}
