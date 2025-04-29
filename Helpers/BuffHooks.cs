using DBTBalanceRevived.Model;
using dbzcalamity;
using dbzcalamity.Buffs.SSJForms;
using DBZMODPORT.Buffs;
using DBZMODPORT.Buffs.SSJBuffs;
using DBZMODPORT.Models;
using DBZMODPORT.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace DBTBalanceRevived.Helpers
{
    public readonly struct DBTBuffInfo
    {
        public readonly float Damage;
        public readonly float Speed;
        public readonly int Defense;
        public readonly float MinDamage;
        public readonly float MaxDamage;
        public readonly float DrainRate;
        public readonly float MasterDrainRate;
        public DBTBuffInfo(float damage, float speed, int defense, float minDamage, float maxDamage, float drainRate, float masterDrainRate)
        {
            Damage = damage;
            Speed = speed;
            Defense = defense;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            DrainRate = drainRate;
            MasterDrainRate = masterDrainRate;
        }
        public DBTBuffInfo(float damage, float speed, int defense, float drainRate, float masterDrainRate)
        {
            Damage = damage;
            Speed = speed;
            Defense = defense;
            DrainRate = drainRate;
            MasterDrainRate = masterDrainRate;
        }
    }
    internal sealed class BuffHooks
    {
        private static Dictionary<Type, DBTBuffInfo> _adjustments;
        public static Dictionary<Type, DBTBuffInfo> Adjustments
        { 
            get
            {
                if (_adjustments is null)
                    InitializeAdjustments();
                return _adjustments;
            }
        }
        public static void RegisterAdjustment<T>(DBTBuffInfo info) where T : ModBuff => _adjustments.Add(typeof(T), info);
        public static void RegisterAdjustment<T>(float damage, float speed, int defense, float drainRate, float masterDrainRate) where T : ModBuff => _adjustments.Add(typeof(T), new DBTBuffInfo(damage, speed, defense, drainRate, masterDrainRate));
        public static void RegisterAdjustment<T>(float damage, float speed, int defense, float minDamage, float maxDamage, float drainRate, float masterDrainRate) where T : ModBuff => _adjustments.Add(typeof(T), new DBTBuffInfo(damage, speed, defense, minDamage, maxDamage, drainRate, masterDrainRate));
        private static void InitializeAdjustments()
        {
            _adjustments = [];

            RegisterAdjustment<SSJ1Buff>(1.20f, 1.10f, 4, 0f, 0f);
            RegisterAdjustment<ASSJBuff>(1.25f, 1.10f, 5, 0f, 0f);
            RegisterAdjustment<USSJBuff>(1.3f, 0.9f, 17, 0f, 0f);
            RegisterAdjustment<SuperKaiokenBuff>(1.3f, 1.10f, 6, 0f, 0f);
            RegisterAdjustment<SSJ2Buff>(1.3f, 1.10f, 11, 0f, 0f);
            RegisterAdjustment<SSJ3Buff>(1.4f, 1.10f, 15, 0f, 0f);
            RegisterAdjustment<SSJGBuff>(1.55f, 1.15f, 23, 0f, 0f);

            RegisterAdjustment<SSJBBuff>(1.70f, 1.30f, 30, 0f, 0f);
            RegisterAdjustment<SSJRBuff>(1.95f, 1.10f, 23, 0f, 0f); // 1.80 dmg if DBCA enabled

            RegisterAdjustment<LSSJBuff>(1.35f, 0.9f, 21, 0f, 0f);
            RegisterAdjustment<LSSJ2Buff>(1.45f, 0.9f, 30, 0f, 0f);
            RegisterAdjustment<LSSJ3Buff>(1.60f, 0.9f, 46, 0f, 0f);

            if (ModLoader.HasMod("dbzcalamity"))
                InitializeAdjustmentsCalamity();
        }
        [JITWhenModsEnabled("dbzcalamity")]
        private static void InitializeAdjustmentsCalamity()
        {
            RegisterAdjustment<PUIBuff>(1.7f, 1.10f, 35, 0f, 0f, 0f, 0f);
            RegisterAdjustment<UEBuff>(2f, 1.15f, 10, 1.85f, 2.5f, 0f, 0f);
        }

        // Replaced with _adjustments - qAngel

        /*
        public static Dictionary<string, (float Damage, float Speed, int defense, float drainRate, float masterDrainRate)> DBT_Adjustments = new()
        {
            { "SSJ1Buff", (1.20f, 1.10f, 4, 0f, 0f) },
            { "ASSJBuff", (1.25f, 1.10f, 5, 0f, 0f) },
            { "USSJBuff", (1.3f, 0.9f, 17, 0f, 0f) },
            { "SuperKaiokenBuff", (1.3f, 1.10f, 6, 0f, 0f) },
            { "SSJ2Buff", (1.3f, 1.10f, 11, 0f, 0f) },
            { "SSJ3Buff", (1.4f, 1.10f, 15, 0f, 0f) },
            { "SSJGBuff", (1.55f, 1.15f, 23, 0f, 0f) },
            
            { "SSJBBuff", (1.70f, 1.30f, 30, 0f, 0f) },
            { "SSJRBuff", (1.95f, 1.10f, 23, 0f, 0f) }, // 1.80 dmg if DBCA enabled

            { "LSSJBuff", (1.35f, 0.9f, 21, 0f, 0f) },
            { "LSSJ2Buff", (1.45f, 0.9f, 30, 0f, 0f) },
            { "LSSJ3Buff", (1.60f, 0.9f, 46, 0f, 0f) },
        };

        public static Dictionary<string, (float Damage, float Speed, int defense, float minDamage, float maxDamage, float drainRate, float masterDrainRate)> DBCA_Adjustments = new()
        {
            { "PUIBuff", (1.7f, 1.10f, 35, 0f, 0f, 0f, 0f) },
            { "UEBuff", (2f, 1.15f, 10, 1.85f, 2.5f, 0f, 0f) }
        };
        */
        public static string BuildTooltip_Hook(TransBuff self)
        {
            BuffInfo kaioken = TransformationHelper.Kaioken;
            BuffInfo superKaioken = TransformationHelper.SuperKaioken;

            string currentDisplayString = string.Empty;
            if (self.Type == kaioken.getBuffId || self.Type == superKaioken.getBuffId)
            {
                switch (self.kaiokenLevel)
                {
                    case 2:
                        currentDisplayString = "(x3)\n";
                        break;
                    case 3:
                        currentDisplayString = "(x4)\n";
                        break;
                    case 4:
                        currentDisplayString = "(x10)\n";
                        break;
                    case 5:
                        currentDisplayString = "(x20)\n";
                        break;
                }
            }
            float dmg = (float)self.damageMulti - 1f;
            float speed = (float)self.speedMulti - 1f;
            float DrainRate = 60f * self.kiDrainRate;
            float DrainRateMastered = 60f * self.kiDrainRateWithMastery;
            float UsageRate = (float)self.attackDrainMulti - 1f;
            int defense = self.baseDefenceBonus;

            StringBuilder sb = new StringBuilder();

            if (dmg != 0)
                sb.Append($"Damage {(dmg>0?"+":"")}{dmg:P2}");
            if (speed != 0)
                sb.AppendLine($"; Speed {(dmg > 0 ? "+" : "")}{speed:P2}");
            if (defense != 0)
                sb.Append($"Defense {(dmg > 0 ? "+" : "")}{defense}");
            if (UsageRate != 0)
                sb.AppendLine($"; Ki Costs {(UsageRate > 0 ? "+" : "")}{UsageRate:P2}");
            if (DrainRate != 0)
                sb.AppendLine($"Ki Drain {MathF.Round(DrainRate):N0}/s, {MathF.Round(DrainRateMastered):N0}/s when mastered");
            if ((int)self.healthDrainRate != 0)
                sb.AppendLine($"Life Drain: -{(int)self.healthDrainRate / 2:N0}/s.");

            if (self.Name == "SSJRBuff" && ModLoader.HasMod("dbzcalamity"))
                sb.AppendLine($"+20% Dodge Chance (Costs 300 Ki).");

            return sb.ToString();
        }

        public delegate void orig_SetStaticDefaults(dynamic self);

        public static void SetStaticDefaults_Hook(orig_SetStaticDefaults orig, dynamic self)
        {
            orig(self);

            if (!BalanceConfigServer.Instance.SSJTweaks)
                return;

            Type t = self.GetType();

            if (Adjustments.TryGetValue(t, out var adjustments))
            {
                if (ModLoader.HasMod("dbzcalamity") && t == typeof(SSJRBuff))
                    self.damageMulti = 1.80f;
                else
                    if (adjustments.Damage != 0)
                        self.damageMulti = adjustments.Damage;
                if (adjustments.Speed != 0)
                    self.speedMulti = adjustments.Speed;
                if (adjustments.Defense != 0)
                    self.baseDefenceBonus = adjustments.Defense;
                if (adjustments.DrainRate != 0)
                    self.kiDrainRate = adjustments.DrainRate;
                if (adjustments.MasterDrainRate != 0)
                    self.kiDrainRateWithMastery = adjustments.MasterDrainRate;
                if (adjustments.MinDamage != 0)
                    self.MinDamage = adjustments.MinDamage;
                if (adjustments.MaxDamage != 0)
                    self.MaxDamage = adjustments.MaxDamage;
            }
        }

        public delegate void orig_Update(dynamic self, Player player, ref int buffIndex);

        public static void Update_Hook(orig_Update orig, dynamic self, Player player, ref int buffIndex)
        {
            orig(self, player, ref buffIndex);
            
            if (!BalanceConfigServer.Instance.SSJTweaks)
                return;

            if(Adjustments.TryGetValue(self.GetType(), out DBTBuffInfo adjustments))
            {
                float dmg = (float)(1.0 + ((adjustments.Damage) - 1.0) * 0.5);

                player.GetDamage(DamageClass.Generic) *= dmg;

                if (ModLoader.HasMod("dbzcalamity") && self.Type == ModContent.BuffType<SSJRBuff>())
                {
                    UpdateCalamity(player);
                }
            }
        }
        [JITWhenModsEnabled("dbzcalamity")]
        private static void UpdateCalamity(Player player)
        {
            dbzcalamityPlayer modPlayer = player.GetModPlayer<dbzcalamityPlayer>();
            modPlayer.dodgeChange += 20f;
        }
    }
}
