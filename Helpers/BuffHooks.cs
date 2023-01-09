using DBTBalance.Model;
using DBZGoatLib.Handlers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DBTBalance.Helpers
{
    internal sealed class BuffHooks
    {
        public static Dictionary<string, (float Damage, float Speed, int defense, float drainRate, float masterDrainRate)> DBT_Adjustments = new()
        {
            { "SSJ1Buff", (1.20f, 1.10f, 9, 0f, 0f) },
            { "ASSJBuff", (1.25f, 1.10f, 14, 0f, 0f) },
            { "USSJBuff", (1.3f, 0.9f, 19, 0f, 0f) },
            { "SuperKaiokenBuff", (1.3f, 1.10f, 12, 0f, 0f) },
            { "SSJ2Buff", (1.3f, 1.10f, 15, 0f, 0f) },
            { "SSJ3Buff", (1.4f, 1.10f, 21, 0f, 0f) },
            { "SSJGBuff", (1.5f, 1.10f, 30, 0f, 0f) },
            
            { "SSJBBuff", (1.75f, 1.30f, 42, 0f, 0f) },
            { "SSJRBuff", (1.95f, 1.10f, 30, 0f, 0f) }, // 1.80 dmg if DBCA enabled

            { "LSSJBuff", (1.35f, 0.9f, 26, 0f, 0f) },
            { "LSSJ2Buff", (1.45f, 0.9f, 43, 0f, 0f) },
            { "LSSJ3Buff", (1.55f, 0.9f, 63, 0f, 0f) },
        };

        public static Dictionary<string, (float Damage, float Speed, int defense, float minDamage, float maxDamage, float drainRate, float masterDrainRate)> DBCA_Adjustments = new()
        {
            { "PUIBuff", (1.7f, 1.10f, 35, 0f, 0f, 0f, 0f) },
            { "UEBuff", (2f, 1.15f, 10, 2f, 2.5f, 0f, 0f) }
        };

        public static string BuildTooltip_Hook(dynamic self)
        {
            var tHelper = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransformationHelper"));

            dynamic kaioken = tHelper.GetProperty("Kaioken").GetValue(null);
            dynamic superKaioken = tHelper.GetProperty("SuperKaioken").GetValue(null);

            string currentDisplayString = string.Empty;
            if (self.Type == (int)kaioken.getBuffId || self.Type == (int)superKaioken.getBuffId)
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
                sb.AppendLine($" | Speed {(dmg > 0 ? "+" : "")}{speed:P2}");
            if (defense != 0)
                sb.Append($"Defense {(dmg > 0 ? "+" : "")}{defense}");
            if (UsageRate != 0)
                sb.AppendLine($" | Ki Costs {(UsageRate > 0 ? "+" : "")}{UsageRate:P2}");
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

            string name = BuffLoader.GetBuff((int)self.Type).Name;

            if (DBT_Adjustments.TryGetValue(name, out var adjustments))
            {
                if (ModLoader.HasMod("dbzcalamity") && name == "SSJRBuff")
                    self.damageMulti = 1.80f;
                else
                    if (adjustments.Damage != 0)
                        self.damageMulti = adjustments.Damage;
                if (adjustments.Speed != 0)
                    self.speedMulti = adjustments.Speed;
                if (adjustments.defense != 0)
                    self.baseDefenceBonus = adjustments.defense;
                if (adjustments.drainRate != 0)
                    self.kiDrainRate = adjustments.drainRate;
                if (adjustments.masterDrainRate != 0)
                    self.kiDrainRateWithMastery = adjustments.masterDrainRate;

                string tip = (string)DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals(name)).GetMethod("AssembleTransBuffDescription").Invoke(self, null);

                self.Description.SetDefault(tip);
            }
            if (ModLoader.HasMod("dbzcalamity"))
            {
                if (DBCA_Adjustments.TryGetValue(name, out var adjustments2))
                {
                    if (adjustments2.Damage != 0)
                        self.damageMulti = adjustments2.Damage;
                    if (adjustments2.Speed != 0)
                        self.speedMulti = adjustments2.Speed;
                    if (adjustments2.defense != 0)
                        self.baseDefenceBonus = adjustments2.defense;
                    if (adjustments2.drainRate != 0)
                        self.kiDrainRate = adjustments2.drainRate;
                    if (adjustments2.masterDrainRate != 0)
                        self.kiDrainRateWithMastery = adjustments2.masterDrainRate;
                    if (adjustments2.minDamage != 0)
                        self.MinDamage = adjustments2.minDamage;
                    if (adjustments2.maxDamage != 0)
                        self.MaxDamage = adjustments2.maxDamage;
                }
            }
        }

        public delegate void orig_Update(dynamic self, Player player, ref int buffIndex);

        public static void Update_Hook(orig_Update orig, dynamic self, Player player, ref int buffIndex)
        {
            orig(self, player, ref buffIndex);
            
            if (!BalanceConfigServer.Instance.SSJTweaks)
                return;

            string name = BuffLoader.GetBuff((int)self.Type).Name;

            if(DBT_Adjustments.TryGetValue(name, out var adjustments))
            {
                float dmg = (float)(1.0 + ((double)(adjustments.Damage) - 1.0) * 0.5);

                player.GetDamage(DamageClass.Generic) *= dmg;

                if(name == "SSJRBuff")
                {
                    var type = DBTBalance.DBCA.Code.DefinedTypes.First(x => x.Name.Equals("dbzcalamityPlayer"));
                    dynamic instance = type.GetMethod("ModPlayer").Invoke(null, new object[] { player });
                    var dodgeChance = type.GetField("dodgeChange");
                    dodgeChance.SetValue(instance, (float)dodgeChance.GetValue(instance) + 20f);
                }
            }
        }
    }
}
