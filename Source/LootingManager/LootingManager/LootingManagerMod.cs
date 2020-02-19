using UnityEngine;
using Verse;

namespace LootingManager
{
    public class LootingManagerMod : Mod
    {
        LootingManagerModSettings lootingManagerModSettings;

        public LootingManagerMod(ModContentPack content) : base(content)
        {
            this.lootingManagerModSettings = GetSettings<LootingManagerModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.Label("Delete chance");
            listingStandard.Gap(listingStandard.verticalSpacing);
            Rect rect1 = listingStandard.GetRect(22f);
            lootingManagerModSettings.deleteChance = Widgets.HorizontalSlider(rect1, lootingManagerModSettings.deleteChance, 0f, 1f, false, (lootingManagerModSettings.deleteChance * 100f).ToString("0") + "%", "0%", "100%", -1f);
            listingStandard.Gap(listingStandard.verticalSpacing);

            listingStandard.CheckboxLabeled("Delete from hostile pawns", ref lootingManagerModSettings.deleteHostile, "Everything this mod offers will get used on hostile pawns");
            listingStandard.CheckboxLabeled("Delete from friendly pawns", ref lootingManagerModSettings.deleteFriendly, "Everything this mod offers will get used on friendly pawns (NOT player faction)");
            listingStandard.CheckboxLabeled("Exclude your prisoners", ref lootingManagerModSettings.excludePrisoners, "Pawns you take as prisoners will NOT be affected by this mod");
            listingStandard.CheckboxLabeled("Delete humanlike corpses (and drop all items)", ref lootingManagerModSettings.deleteCorpses, "If a pawn dies, it will drop everything and then the naked corpse will disappear");
            listingStandard.CheckboxLabeled("Delete only unresearched things", ref lootingManagerModSettings.deleteOnlyUnresearched, "Only things that the player has not researched will be affected by this mod");
            listingStandard.CheckboxLabeled("Delete weapons", ref lootingManagerModSettings.deleteWeapons, "Weapons will get deleted if dropped");
            listingStandard.CheckboxLabeled("Delete apparel", ref lootingManagerModSettings.deleteApparel, "Apparel will get deleted if dropped");
            listingStandard.CheckboxLabeled("Delete everything else", ref lootingManagerModSettings.deleteEverythingElse, "Everything that is not a weapon or apparel will get deleted if dropped");
            listingStandard.CheckboxLabeled("Disassemble items instead of deleting", ref lootingManagerModSettings.refundItems, "Deleted items will spawn their crafting ingrediens (if they have any)");

            listingStandard.Gap(listingStandard.verticalSpacing);
            listingStandard.Label("Disassemble efficiency (not used in vanilla Rimworld)", -1f, "Vanilla Rimworld does not use efficiency when disassembling items, but it may be used in some mods");
            listingStandard.Gap(listingStandard.verticalSpacing);
            Rect rect2 = listingStandard.GetRect(22f);
            lootingManagerModSettings.refundEfficiency = Widgets.HorizontalSlider(rect2, lootingManagerModSettings.refundEfficiency, 0f, 1f, false, (lootingManagerModSettings.refundEfficiency*100f).ToString("0") + "%", "0%", "100%", -1f);
            listingStandard.Gap(listingStandard.verticalSpacing);

            listingStandard.End();
        }

        public override string SettingsCategory()
        {
            return "Looting Manager";
        }
    }
}
