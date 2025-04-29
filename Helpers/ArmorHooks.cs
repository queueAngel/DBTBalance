using dbzcalamity.Items.Armor.ArmorSets;
using dbzcalamity.Items.Armor.CalamityHelmets;
using DBZMODPORT.Items.Armor;
using DBZMODPORT.Items.Armor.ArmorSets;
using System;
using Terraria.ModLoader;

namespace DBTBalanceRevived.Helpers
{
    internal sealed class ArmorHooks
    {
        internal static Type[] ArmorTypes
        {
            get
            {
                Type[] types =
                [
                    typeof(AdamantiteVisor),
                    typeof(ChlorophyteHeadpiece),
                    typeof(CobaltCrown),
                    typeof(HallowedFedora),
                    typeof(MythrilGlasses),
                    typeof(OrichalcumHat),
                    typeof(PalladiumBlindfoldCrown),
                    typeof(TitaniumCap),
                    typeof(BlackFusionPants),
                    typeof(BlackFusionShirt),
                    typeof(DemonLeggings),
                    typeof(DemonShirt),
                    typeof(EliteSaiyanBreastplate),
                    typeof(EliteSaiyanLeggings),
                    typeof(RadiantGreaves),
                    typeof(RadiantScalepiece),
                    typeof(RadiantVisor),
                    typeof(SaiyanBreastplate)
                ];

                if (ModLoader.HasMod("dbzcalamity"))
                    return [.. types, ..ArmorTypesCalamity];
                return types;
            }
        }
        [JITWhenModsEnabled("dbzcalamity")]
        private static Type[] ArmorTypesCalamity
        {
            get
            {
                return
                [
                    typeof(AngelGleaves),
                    typeof(AngelRing),
                    typeof(AngelRobes),
                    typeof(DestructionGodBottoms),
                    typeof(DestructionGodTop),
                    typeof(DragonHelm),
                    typeof(DragonScalelegs),
                    typeof(DragonScalemail),
                    typeof(EnoflameBand),
                    typeof(EnoflameBreastplate),
                    typeof(EnoflameLeggings),
                    typeof(LegendaryDragonHelm),
                    typeof(LegendaryDragonScalelegs),
                    typeof(LegendaryDragonScalemail),
                    typeof(AuricTeslaBattleBand),
                    typeof(BloodflareDemonHead),
                    typeof(DaedalusBand),
                    typeof(GodSlayerHood),
                    typeof(HydrothermicBand),
                    typeof(ReaverHood),
                    typeof(SilvaHeadgear),
                    typeof(StatigelBand),
                    typeof(TarragonHood)
                ];
            }
        }

        public delegate void Orig_SetStaticDefaults_Armor(dynamic self);

        public static void Armor_SetStaticDefaults(Orig_SetStaticDefaults_Armor orig, dynamic self)
        {
            orig(self);

            if (self.Item.defense > 0)
                self.Item.defense -= (int)(self.Item.defense * 0.30f);
        }
    }
}
