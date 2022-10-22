using Verse;
using HarmonyLib;
using RimWorld;
using System;

namespace LootingManager
{
    [StaticConstructorOnStartup]
    static class HarmonyPatch
    {
        static HarmonyPatch()
        {
            var harmony = new Harmony("rimworld.carnysenpai.lootingmanager");
            harmony.Patch(AccessTools.Method(typeof(Pawn), "Kill"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Kill_PostFix")), null);
            harmony.Patch(AccessTools.Method(typeof(ThingOwner), "TryDrop", new[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(ThingPlaceMode), typeof(int), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>), typeof(Predicate<IntVec3>) }), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryDrop_PostFix")), null);
            harmony.Patch(AccessTools.Method(typeof(ThingOwner), "TryDrop", new[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>), typeof(Predicate<IntVec3>), typeof(bool) }), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryDrop_PostFix")), null);
        }

        [HarmonyPostfix]
        public static void Kill_PostFix(Pawn __instance)
        {
            if (__instance == null) return;
            if (__instance.Dead && (__instance.RaceProps.Humanlike || __instance.RaceProps.Animal) && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteCorpses)
            {
                if (__instance.Faction == Faction.OfPlayer) return;
                if (__instance.RaceProps.Animal && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteAnimals) return;
                if (__instance.IsPrisonerOfColony && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().excludePrisoners) return;
                if (__instance.Faction.HostileTo(Faction.OfPlayer) && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteHostile) return;
                if (!__instance.Faction.HostileTo(Faction.OfPlayer) && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteFriendly) return;
                if (__instance.Spawned) __instance.Strip();
                if (__instance.Corpse.Spawned && !__instance.Corpse.Destroyed) __instance.Corpse.Destroy(DestroyMode.Vanish);
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
            if (holdingPawn.RaceProps.Animal && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteAnimals) return;
            if (!holdingPawn.Faction.HostileTo(Faction.OfPlayer) && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteFriendly) return;

            if (thing.def.IsWeapon && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteWeapons) return;
            if (thing.def.IsApparel && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteApparel) return;
            if (!thing.def.IsWeapon && !thing.def.IsApparel && !LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteEverythingElse) return;
            if (!holdingPawn.Dead && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteOnlyFromCorpses) return;

            bool researchFinished = true;
            if (thing.def.researchPrerequisites != null)
            {
                researchFinished = thing.def.IsResearchFinished;
            }
            else if (thing.def.recipeMaker == null)
            {
                researchFinished = true;
            }
            else if (thing.def.recipeMaker.researchPrerequisite == null && thing.def.recipeMaker.researchPrerequisites == null)
            {
                researchFinished = true;
            }
            else
            {
                if (thing.def.recipeMaker.researchPrerequisites == null)
                {
                    researchFinished = thing.def.recipeMaker.researchPrerequisite.IsFinished;
                }
                else
                {
                    researchFinished = !thing.def.recipeMaker.researchPrerequisites.Any(r => !r.IsFinished);
                }
            }            
            if (LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteOnlyUnresearched && researchFinished) return;

            if (!Rand.Chance(LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().deleteChance)) return;

            if (LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().refundItems)
            {
                foreach (Thing product in thing.SmeltProducts(LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().refundEfficiency))
                {
                    ThingWithComps refundedThing = (ThingWithComps)ThingMaker.MakeThing(product.def);
                    refundedThing.stackCount = Math.Min(product.stackCount, product.def.stackLimit);
                    if (refundedThing.stackCount > 0) GenSpawn.Spawn(refundedThing, holdingPawn.PositionHeld, holdingPawn.MapHeld);
                }
            }
            CompReloadable compReloadable = thing.TryGetComp<CompReloadable>();
            if (compReloadable != null)
            {
                int chargesCount = 0;
                while (compReloadable.RemainingCharges > 0)
                {
                    compReloadable.UsedOnce();
                    chargesCount++;
                }
                if (compReloadable.AmmoDef != null && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().ejectAmmo)
                {
                    ThingWithComps refundedThing = (ThingWithComps)ThingMaker.MakeThing(compReloadable.AmmoDef);
                    refundedThing.stackCount = Math.Min(chargesCount * compReloadable.Props.ammoCountPerCharge, compReloadable.AmmoDef.stackLimit);
                    if (refundedThing.stackCount > 0) GenSpawn.Spawn(refundedThing, holdingPawn.PositionHeld, holdingPawn.MapHeld);
                }
                else if (compReloadable.AmmoDef != null && LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().refundItems)
                {
                    ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(compReloadable.AmmoDef);
                    thingWithComps.stackCount = Math.Min(chargesCount * compReloadable.Props.ammoCountPerCharge, compReloadable.AmmoDef.stackLimit);
                    foreach (Thing product in thingWithComps.SmeltProducts(LoadedModManager.GetMod<LootingManagerMod>().GetSettings<LootingManagerModSettings>().refundEfficiency))
                    {
                        ThingWithComps refundedThing = (ThingWithComps)ThingMaker.MakeThing(product.def);
                        refundedThing.stackCount = Math.Min(product.stackCount, product.def.stackLimit);
                        if (refundedThing.stackCount > 0) GenSpawn.Spawn(refundedThing, holdingPawn.PositionHeld, holdingPawn.MapHeld);
                    }
                }
                
            }
            if (!thing.Destroyed) thing.Destroy(DestroyMode.Vanish);
        }
    }
}
