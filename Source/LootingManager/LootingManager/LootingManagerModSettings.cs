using Verse;

namespace LootingManager
{
    public class LootingManagerModSettings : ModSettings
    {
        public float deleteChance = 1f;
        public bool deleteHostile = true;
        public bool deleteFriendly = false;
        public bool excludePrisoners = true;
        public bool deleteCorpses = false;
        public bool deleteOnlyUnresearched = false;
        public bool deleteWeapons = true;
        public bool deleteApparel = false;
        public bool ejectAmmo = true;
        public bool deleteEverythingElse = false;
        public bool deleteOnlyFromCorpses = false;
        public bool refundItems = false;
        public float refundEfficiency = 0.5f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref deleteChance, "DeleteChance", 1f, false);
            Scribe_Values.Look<bool>(ref deleteHostile, "DeleteHostile", true, false);
            Scribe_Values.Look<bool>(ref deleteFriendly, "DeleteFriendly", false, false);
            Scribe_Values.Look<bool>(ref excludePrisoners, "ExcludePrisoners", true, false);
            Scribe_Values.Look<bool>(ref deleteCorpses, "DeleteCorpses", false, false);
            Scribe_Values.Look<bool>(ref deleteOnlyUnresearched, "DeleteOnlyUnresearched", false, false);
            Scribe_Values.Look<bool>(ref deleteWeapons, "deleteWeapons", true, false);
            Scribe_Values.Look<bool>(ref deleteApparel, "deleteApparel", false, false);
            Scribe_Values.Look<bool>(ref ejectAmmo, "ejectAmmo", true, false);
            Scribe_Values.Look<bool>(ref deleteEverythingElse, "deleteEverythingElse", false, false);
            Scribe_Values.Look<bool>(ref deleteOnlyFromCorpses, "deleteOnlyFromCorpses", false, false);
            Scribe_Values.Look<bool>(ref refundItems, "RefundItems", false, false);
            Scribe_Values.Look<float>(ref refundEfficiency, "RefundEfficiency", 0.5f, false);
        }
    }
}
