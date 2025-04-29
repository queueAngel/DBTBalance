using dbzcalamity.Items.Accessories;
using dbzcalamity.Items.Accessories.BossDrops;
using dbzcalamity.Items.Accessories.ZenkaiUpgraders;
using DBZMODPORT;
using DBZMODPORT.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace DBTBalanceRevived.Helpers
{
    internal static class AccessoryHooks
    {
        public static Dictionary<ushort, ushort[]> UpgradePaths { get; private set; }
        public static Dictionary<ushort, float> DamageAdjustment { get; private set; }
        internal static void Initialize()
        {
            InitializeUpgradePaths();
            InitializeDamageAdjustment();
        }
        public static Type[] UpgradeTypes
        { 
            get
            {
                Type[] types =
                [
                    typeof(ArmCannon),
                    typeof(WornGloves),
                    typeof(ScouterT2),
                    typeof(ScouterT3),
                    typeof(ScouterT4),
                    typeof(ScouterT5),
                    typeof(CrystalliteFlow),
                    typeof(CrystalliteControl),
                    typeof(BaldurEssentia),
                    typeof(LargeTurtleShell),
                    typeof(BurningEnergyAmulet),
                    typeof(IceTalisman),
                    typeof(EarthenSigil),
                    typeof(EarthenScarab),
                    typeof(KiChip),
                    typeof(SpiritualEmblem),
                    typeof(DragonGemNecklace),
                    typeof(AmberNecklace),
                    typeof(AmethystNecklace),
                    typeof(DiamondNecklace),
                    typeof(EmeraldNecklace),
                    typeof(RubyNecklace),
                    typeof(SapphireNecklace),
                    typeof(TopazNecklace),
                ];
                if (ModLoader.HasMod("dbzcalamity"))
                    return [.. types, .. UpgradeTypesCalamity];
                return types;
            } 
        }
        [JITWhenModsEnabled("dbzcalamity")]
        internal static Type[] UpgradeTypesCalamity
        {
            get
            {
                return
                [
                    // vanilla entries that are only added with calamity
                    typeof(BattleKit),
                    typeof(BloodstainedBandana),
                    typeof(BlackDiamondShell),
                    // new entries
                    typeof(FreezingBandana),
                    typeof(BurningBandana),
                    typeof(RainbowBandana),
                    typeof(ShadowEye),
                    typeof(HemoTouch),
                    typeof(ExtraPair),
                    typeof(PhantomShield)
                ];
            }
        }
        #region Params T[] When?
        public static void RegisterUpgradePath<TSource, TUpgradeA>()
            where TSource : ModItem
            where TUpgradeA : ModItem => UpgradePaths[(ushort)ItemType<TSource>()] = [(ushort)ItemType<TUpgradeA>()];
        public static void RegisterUpgradePath<TSource, TUpgradeA, TUpgradeB>()
            where TSource : ModItem
            where TUpgradeA : ModItem
            where TUpgradeB : ModItem => UpgradePaths[(ushort)ItemType<TSource>()] = [(ushort)ItemType<TUpgradeA>(), (ushort)ItemType<TUpgradeB>()];
        public static void RegisterUpgradePath<TSource, TUpgradeA, TUpgradeB, TUpgradeC>()
            where TSource : ModItem
            where TUpgradeA : ModItem
            where TUpgradeB : ModItem
            where TUpgradeC : ModItem => UpgradePaths[(ushort)ItemType<TSource>()] = [(ushort)ItemType<TUpgradeA>(), (ushort)ItemType<TUpgradeB>(), (ushort)ItemType<TUpgradeC>()];
        public static void RegisterUpgradePath<TSource, TUpgradeA, TUpgradeB, TUpgradeC, TUpgradeD, TUpgradeE>()
    where TSource : ModItem
    where TUpgradeA : ModItem
    where TUpgradeB : ModItem
    where TUpgradeC : ModItem
    where TUpgradeD : ModItem
    where TUpgradeE : ModItem => UpgradePaths[(ushort)ItemType<TSource>()] = [(ushort)ItemType<TUpgradeA>(), (ushort)ItemType<TUpgradeB>(), (ushort)ItemType<TUpgradeC>(), (ushort)ItemType<TUpgradeD>(), (ushort)ItemType<TUpgradeE>()];
        public static void RegisterUpgradePath<TSource, TUpgradeA, TUpgradeB, TUpgradeC, TUpgradeD, TUpgradeE, TUpgradeF>()
            where TSource : ModItem
            where TUpgradeA : ModItem
            where TUpgradeB : ModItem
            where TUpgradeC : ModItem
            where TUpgradeD : ModItem
            where TUpgradeE : ModItem
            where TUpgradeF : ModItem => UpgradePaths[(ushort)ItemType<TSource>()] = [(ushort)ItemType<TUpgradeA>(), (ushort)ItemType<TUpgradeB>(), (ushort)ItemType<TUpgradeC>(), (ushort)ItemType<TUpgradeD>(), (ushort)ItemType<TUpgradeE>(), (ushort)ItemType<TUpgradeF>()];
        #endregion
        public static void RegisterDamageAdjustment<TTarget>(float adjustment) where TTarget : ModItem => DamageAdjustment.Add((ushort)ItemType<TTarget>(), adjustment);
        internal static void InitializeDamageAdjustment()
        {
            DamageAdjustment = [];

            RegisterDamageAdjustment<BlackDiamondShell>(0.02f); // 12% > 10%
            RegisterDamageAdjustment<BloodstainedBandana>(0.10f); // 14% > 4%
            RegisterDamageAdjustment<BurningEnergyAmulet>(0.05f); // 0%
            RegisterDamageAdjustment<EarthenArcanium>(0.10f); // 0%
            RegisterDamageAdjustment<EarthenScarab>(0.01f); // 0%
            RegisterDamageAdjustment<EarthenSigil>(0.06f); // 0%
            RegisterDamageAdjustment<IceTalisman>(0.05f); // 7% > 2%
            RegisterDamageAdjustment<LuminousSectum>(0.05f); // 4%
            RegisterDamageAdjustment<MetamoranSash>(0.05f); // 5%
            RegisterDamageAdjustment<PureEnergyCirclet>(0.07f); // 5%
            RegisterDamageAdjustment<RadiantTotem>(0.12f); // 0%
            RegisterDamageAdjustment<ZenkaiCharm>(0.08f); // 0%

            if (ModLoader.HasMod("dbzcalamity"))
                DamageAdjustmentCalamity();
        }
        [JITWhenModsEnabled("dbzcalamity")]
        private static void DamageAdjustmentCalamity()
        {
            RegisterDamageAdjustment<AviationBoots>(0.04f); // 0%
            RegisterDamageAdjustment<BurningBandana>(0.08f); // 12% > 4%
            RegisterDamageAdjustment<FreezingBandana>(0.08f); // 12% > 4%
            RegisterDamageAdjustment<DiamondStorm>(0.02f); // 12% > 10%
            RegisterDamageAdjustment<ZenkaiOrb>(0.1f); // 0%
            RegisterDamageAdjustment<ZenkaiAmulet>(0.12f); // 0%
            RegisterDamageAdjustment<ElysianCrystal>(0.04f); // 0%
            RegisterDamageAdjustment<PhantomShield>(0.06f); // 11% > 5%
            RegisterDamageAdjustment<ZenkaiRing>(0.16f); // 0%
            RegisterDamageAdjustment<ZenkaiCrown>(0.21f); // 0%
            RegisterDamageAdjustment<FinalShield>(0.04f); // 19% > 15%
        }
        internal static void InitializeUpgradePaths()
        {
            UpgradePaths = [];

            // ArmCannon
            RegisterUpgradePath<ArmCannon, BattleKit>();
            // WornGloves
            RegisterUpgradePath<WornGloves, BattleKit>();
            // ScouterT2 
            RegisterUpgradePath<ScouterT2, BattleKit, ScouterT3, ScouterT4, ScouterT5, ScouterT6>();
            // ScouterT3
            RegisterUpgradePath<ScouterT3, ScouterT4, ScouterT5, ScouterT6>();
            // ScouterT4
            RegisterUpgradePath<ScouterT4, ScouterT5, ScouterT6>();
            // ScouterT5
            RegisterUpgradePath<ScouterT5, ScouterT6>();
            // CrystalliteFlow
            RegisterUpgradePath<CrystalliteFlow, CrystalliteAlleviate>();
            // CrystalliteControl
            RegisterUpgradePath<CrystalliteControl, CrystalliteAlleviate, CrystalliteFlow>();
            // BaldurEssentia
            RegisterUpgradePath<BaldurEssentia, BuldariumSigmite>();
            // LargeTurtleShell
            RegisterUpgradePath<LargeTurtleShell, BlackDiamondShell>();
            // BurningEnergyAmulet
            RegisterUpgradePath<BurningEnergyAmulet, PureEnergyCirclet>();
            // IceTalisman
            RegisterUpgradePath<IceTalisman, PureEnergyCirclet>();
            // EarthenSigil
            RegisterUpgradePath<EarthenSigil, EarthenArcanium>();
            // EarthenScarab
            RegisterUpgradePath<EarthenScarab, EarthenArcanium>();
            // KiChip
            RegisterUpgradePath<KiChip, BuldariumSigmite>();
            // SpiritualEmblem
            RegisterUpgradePath<SpiritualEmblem, SpiritCharm>();
            // DragonGemNecklace
            RegisterUpgradePath<DragonGemNecklace, SpiritCharm>();
            // AmberNecklace
            RegisterUpgradePath<AmberNecklace, DragonGemNecklace, SpiritCharm>();
            // AmethystNecklace
            RegisterUpgradePath<AmethystNecklace, DragonGemNecklace, SpiritCharm>();
            // DiamondNecklace
            RegisterUpgradePath<DiamondNecklace, DragonGemNecklace, SpiritCharm>();
            // EmeraldNecklace
            RegisterUpgradePath<EmeraldNecklace, DragonGemNecklace, SpiritCharm>();
            // RubyNecklace
            RegisterUpgradePath<RubyNecklace, DragonGemNecklace, SpiritCharm>();
            // SapphireNecklace
            RegisterUpgradePath<SapphireNecklace, DragonGemNecklace, SpiritCharm>();
            // TopazNecklace
            RegisterUpgradePath<TopazNecklace, DragonGemNecklace, SpiritCharm>();

            if (ModLoader.HasMod("dbzcalamity"))
                UpgradePathsCalamity();
        }
        [JITWhenModsEnabled("dbzcalamity")]
        private static void UpgradePathsCalamity()
        {
            // update entries

            // ArmCannon
            RegisterUpgradePath<ArmCannon, BattleKit, FinalShield>();
            // WornGloves
            RegisterUpgradePath<WornGloves, BattleKit, FinalShield>();
            // ScouterT2
            RegisterUpgradePath<ScouterT2, BattleKit, ScouterT3, ScouterT4, ScouterT5, ScouterT6, FinalShield>();
            // LargeTurtleShell
            RegisterUpgradePath<LargeTurtleShell, BlackDiamondShell, DiamondStorm>();

            // new entries (for vanilla)

            // BattleKit
            RegisterUpgradePath<BattleKit, FinalShield>();
            // BloodstainedBanadana
            RegisterUpgradePath<BloodstainedBandana, FrostburnBandana, RainbowBandana, RingOfAll>();
            // BlackDiamondShell
            RegisterUpgradePath<BlackDiamondShell, DiamondStorm>();

            // new entries (original)

            // FreezingBandana
            RegisterUpgradePath<FreezingBandana, FrostburnBandana, RainbowBandana, RingOfAll>();
            // BurningBandana
            RegisterUpgradePath<BurningBandana, FrostburnBandana, RainbowBandana, RingOfAll>();
            // RainbowBandana
            RegisterUpgradePath<RainbowBandana, RingOfAll>();
            // ShadowEye
            RegisterUpgradePath<ShadowEye, Scheusslich>();
            // HemoTouch
            RegisterUpgradePath<HemoTouch, Scheusslich>();
            // ExtraPair
            RegisterUpgradePath<ExtraPair, Scheusslich>();
            // PhantomShield
            RegisterUpgradePath<PhantomShield, FinalShield>();
        }

        public static bool HasAccessory(Player player, int type)
        {
            if (!UpgradePaths.TryGetValue((ushort)type, out var upgrades))
                return false;
            return player.TryGetModPlayer(out BPlayer modPlayer) && modPlayer.upgradePathBuffer != null && modPlayer.upgradePathBuffer.Intersect(upgrades).Any();
        }

        public delegate void orig_Update(dynamic self, Player player, bool hideVisual);

        public static void Update_hook(orig_Update orig, dynamic self, Player player, bool hideVisual)
        {
            if (self is null || HasAccessory(player, self.Type))
                return;

            orig(self, player, hideVisual);

            if(DamageAdjustment.TryGetValue((ushort)self.Type, out float amount))
            {
                player.GetModPlayer<MyPlayer>().kiDamage -= amount;
                //player.GetDamage<KiDamageType>() -= amount;
            }
        }
    }
}
