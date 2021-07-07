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

            listingStandard.Label("lootingManagerDeleteChanceLabel".Translate(), -1f, "lootingManagerDeleteChanceDescription".Translate());
            listingStandard.Gap(listingStandard.verticalSpacing);
            Rect rect1 = listingStandard.GetRect(22f);
            lootingManagerModSettings.deleteChance = Widgets.HorizontalSlider(rect1, lootingManagerModSettings.deleteChance, 0f, 1f, false, (lootingManagerModSettings.deleteChance * 100f).ToString("0") + "%", "0%", "100%", -1f);
            listingStandard.Gap(listingStandard.verticalSpacing);

            listingStandard.CheckboxLabeled("lootingManagerDeleteHostileLabel".Translate(), ref lootingManagerModSettings.deleteHostile, "lootingManagerDeleteHostileDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerDeleteFriendlyLabel".Translate(), ref lootingManagerModSettings.deleteFriendly, "lootingManagerDeleteFriendlyDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerExcludePrisonersLabel".Translate(), ref lootingManagerModSettings.excludePrisoners, "lootingManagerExcludePrisonersDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerDeleteCorpsesLabel".Translate(), ref lootingManagerModSettings.deleteCorpses, "lootingManagerDeleteCorpsesDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerDeleteOnlyUnresearchedLabel".Translate(), ref lootingManagerModSettings.deleteOnlyUnresearched, "lootingManagerDeleteOnlyUnresearchedDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerDeleteWeaponsLabel".Translate(), ref lootingManagerModSettings.deleteWeapons, "lootingManagerDeleteWeaponsDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerDeleteApparelLabel".Translate(), ref lootingManagerModSettings.deleteApparel, "lootingManagerDeleteApparelDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerEjectAmmoLabel".Translate(), ref lootingManagerModSettings.ejectAmmo, "lootingManagerEjectAmmoDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerDeleteEverythingElseLabel".Translate(), ref lootingManagerModSettings.deleteEverythingElse, "lootingManagerDeleteEverythingElseDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerDeleteOnlyFromCorpsesLabel".Translate(), ref lootingManagerModSettings.deleteOnlyFromCorpses, "lootingManagerDeleteOnlyFromCorpsesDescription".Translate());
            listingStandard.CheckboxLabeled("lootingManagerRefundItemsLabel".Translate(), ref lootingManagerModSettings.refundItems, "lootingManagerRefundItemsDescription".Translate());

            listingStandard.Gap(listingStandard.verticalSpacing);
            listingStandard.Label("lootingManagerRefundEfficiencyLabel".Translate(), -1f, "lootingManagerRefundEfficiencyDescription".Translate());
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
