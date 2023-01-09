using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTBalance.Helpers
{
    internal sealed class ArmorHooks
    {
        internal static List<string> DBT_Armor = new()
        {
            "AdamantiteVisor",
            "ChlorophyteHeadpiece",
            "CobaltCrown",
            "HallowedFedora",
            "MythrilGlasses",
            "OrichalcumHat",
            "PalladiumBlindfoldCrown",
            "TitaniumCap",
            "BlackFusionPants",
            "BlackFusionShirt",
            "DemonLeggings",
            "DemonShirt",
            "EliteSaiyanBreastplate",
            "EliteSaiyanLeggings",
            "RadiantGreaves",
            "RadiantScalepiece",
            "RadiantVisor",
            "SaiyanBreastplate"
        };

        internal static List<string> DBCA_Armor = new()
        {
            "AngelGleaves",
            "AngelRing",
            "AngelRobes",
            "DestructionGodBottoms",
            "DestructionGodTop",
            "DragonHelm",
            "DragonScalelegs",
            "DragonScalemail",
            "EnoflameBand",
            "EnoflameBreastplate",
            "EnoflameLeggings",
            "LegendaryDragonHelm",
            "LegendaryDragonScalelegs",
            "LegendaryDragonScalemail",
            "AuricTeslaBattleBand",
            "BloodflareDemonHead",
            "DaedalusBand",
            "GodSlayerHood",
            "HydrothermicBand",
            "ReaverHood",
            "SilvaHeadgear",
            "StatigelBand",
            "TarragonHood"
        };

        public delegate void Orig_SetStaticDefaults_Armor(dynamic self);

        public static void Armor_SetStaticDefaults(Orig_SetStaticDefaults_Armor orig, dynamic self)
        {
            orig(self);

            if ((int)self.Item.defense > 0) ;
                self.Item.defense -= (int)((int)self.Item.defense * 0.30f);
        }
    }
}
