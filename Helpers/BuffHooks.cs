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
    internal class BuffHooks
    {
        public static Dictionary<string, (float Damage, float Speed, int defense, float drainRate, float masterDrainRate)> DBT_Adjustments = new()
        {
            { "SSJ1Buff", (1.20f, 1.2f, 4, 0f, 0f) },
            { "ASSJBuff", (1.25f, 1.2f, 5, 0f, 0f) },
            { "USSJBuff", (1.28f, 0.9f, 7, 0f, 0f) },
            { "SuperKaiokenBuff", (1.29f, 1.2f, 7, 0f, 0f) },
            { "SSJ2Buff", (1.3f, 1.24f, 8, 0f, 0f) },
            { "LSSJBuff", (1.35f, 1.3f, 6, 0f, 0f) },
            { "SSJ3Buff", (1.4f, 1.3f, 12, 0f, 0f) },
            { "LSSJ2Buff", (1.5f, 1.35f, 14, 0f, 0f) },
            { "SSJGBuff", (1.6f, 1.35f, 16, 0f, 0f) },
            { "LSSJ3Buff", (1.65f, 1.4f, 20, 0f, 0f) },
            { "SSJBBuff", (1.75f, 1.55f, 23, 0f, 0f) },
            { "SSJRBuff", (1.85f, 1.4f, 17, 0f, 0f) }
        };

        public static Dictionary<string, (float Damage, float Speed, int defense, float dodgeBonus, float minDamage, float maxDamage, float drainRate, float masterDrainRate)> DBCA_Adjustments = new()
        {
            { "UISignBuff", (1.9f, 1.4f, 20, 0.2f, 0f, 0f, 0f, 0f) },
            { "UIBuff", (2f, 1.55f, 22, 0.2f, 0f, 0f, 0f, 0f) },
            { "UEBuff", (2.2f, 1.3f, 18, 0f, 2f, 2.5f, 0f, 0f) }
        };

        public delegate void orig_SetStaticDefaults(dynamic self);

        public static void SetStaticDefaults_Hook(orig_SetStaticDefaults orig, dynamic self)
        {
            orig(self);

            if (!BalanceConfigServer.Instance.SSJTweaks)
                return;

            string name = BuffLoader.GetBuff((int)self.Type).Name;

            if (DBT_Adjustments.TryGetValue(name, out var adjustments))
            {
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
                        self.minDamage = adjustments2.minDamage;
                    if (adjustments2.maxDamage != 0)
                        self.maxDamage = adjustments2.maxDamage;
                    if (adjustments2.dodgeBonus != 0)
                        self.dodgeChance = adjustments2.dodgeBonus;
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
            }
        }

        public static void DBCA_Update_Hook(dynamic self, Player player, ref int buffIndex)
        {
            string name = BuffLoader.GetBuff((int)self.Type).Name;

            var dbcPlayerClass = DBTBalance.DBCA.Code.DefinedTypes.First(x => x.Name.Equals("dbzcalamityPlayer"));
            dynamic dbcPlayer = dbcPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { player });
            bool IsAngelic = (bool)dbcPlayerClass.GetMethod("IsPlayerAngelic").Invoke(dbcPlayer, null);
            var stopAuraSound = dbcPlayerClass.GetMethod("StopAuraSound");
            var puiActive = dbcPlayerClass.GetField("puiActive");
            var uiActive = dbcPlayerClass.GetField("uiActive");

            dynamic modPlayer = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetMethod("ModPlayer").Invoke(null, new object[] { player });
            var KiDamage = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetField("KiDamage");
            var kiDrainMulti = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetField("kiDrainMulti");

            
            if (TransformationHandler.IsAnythingBut(player,self.Type,true))
            {
                TransformationHandler.ClearTransformations(player);
                return;
            }

            Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(Utils.NextFloat(Main.rand, (float)(-(float)player.width - 2), (float)(player.width + 2)), Utils.NextFloat(Main.rand, (float)(-(float)player.height - 6), (float)(player.height + 6))), 221, new Vector2?(Utils.RotatedByRandom(new Vector2(0f, -1.5f), (double)MathHelper.ToRadians(80f))), 0, default(Color), 1f);
            Lighting.AddLight(player.Center, Color.Blue.ToVector3());
            dust.noGravity = true;
            if (name == "UIBuff")
            {
                Dust dust2 = Dust.NewDustPerfect(player.Center + new Vector2(Utils.NextFloat(Main.rand, (float)(-(float)player.width - 6), (float)(player.width + 6)), Utils.NextFloat(Main.rand, (float)(-(float)player.height - 6), (float)(player.height + 6))), 70, new Vector2?(Utils.RotatedByRandom(new Vector2(0f, -1.5f), (double)MathHelper.ToRadians(100f))), 0, default(Color), 1f);
                Lighting.AddLight(player.Center, Color.Purple.ToVector3());
                dust2.noGravity = true;
            }
            if (player.HasBuff(DBTBalance.DBZMOD.Find<ModBuff>("TiredDebuff").Type))
            {
                player.GetDamage(DamageClass.Melee) /= 0.8f;
                player.GetDamage(DamageClass.Ranged) /= 0.8f;
                player.GetDamage(DamageClass.Magic) /= 0.8f;
                player.GetDamage(DamageClass.Summon) /= 0.8f;
                player.GetDamage(DamageClass.Throwing) /= 0.8f;
                KiDamage.SetValue(modPlayer, modPlayer.KiDamage / .8f);
            }
            float speedMulti = self.speedMulti;
            float damageMulti = self.damageMulti;
            float attackDrainMulti = self.attackDrainMulti;
            int baseDefenceBonus = self.baseDefenceBonus;
            float kiDrainRate = self.kiDrainRate;

            if (name == "UEBuff")
            {
                float maxDamage = self.maxDamage ?? 0f;
                float minDamage = self.minDamage ?? 0f;

                var ueDamageBonusPerDamageTaken = dbcPlayerClass.GetField("ueDamageBonusPerDamageTaken");

                ueDamageBonusPerDamageTaken.SetValue(dbcPlayer, self.damageBonusPerDamageTaken ?? 0f);

                self.baseMaxDamage = maxDamage;

                if ((bool)dbcPlayerClass.GetField("destructionUEboostBuff").GetValue(dbcPlayer))
                {
                    maxDamage += 0.1f;
                    self.ueDamageBonusPerDamageTaken = self.baseBonusPerDamage + self.baseBonusPerDamage / 2f;
                }
                else
                {
                    self.maxDamage = self.baseMaxDamage;

                }

                self.damageBonusPerDamageTaken = self.baseBonusPerDamage;

                damageMulti = Math.Min(self.maxDamage, self.minDamage + (float)dbcPlayerClass.GetField("ueCurrentDamageBonus").GetValue(dbcPlayer));
            }
            else
            {
                float dodgeChance = self.dodgeChance ?? 0f;
                var dodgechance = dbcPlayerClass.GetField("dodgeChange");
                dodgechance.SetValue(dbcPlayer, (float)dodgechance.GetValue(dbcPlayer) + dodgeChance);
            }

            if (IsAngelic)
            {
                damageMulti += 0.1f;
                speedMulti += 0.03f;
            }

            if (modPlayer.IsKiDepleted())
            {
                stopAuraSound.Invoke(dbcPlayer, null);
                player.DelBuff(buffIndex);
                buffIndex--;
                puiActive.SetValue(dbcPlayer, false);
                uiActive.SetValue(dbcPlayer, false);
            }
            else
            {
                if (name == "UISignBuff")
                    modPlayer.AddKi(self.kiDrainRate * -1f, false, true);

                if (name == "UIBuff")
                    modPlayer.AddKi(self.kiDrainRate * -1f, false, true);
            }

            player.statDefense += baseDefenceBonus;

            modPlayer.AddKi(kiDrainRate * -1f, false, true);
            Lighting.AddLight(player.Center, 1f, 1f, 0f);

            player.moveSpeed *= speedMulti * modPlayer.bonusSpeedMultiplier;
            player.maxRunSpeed *= speedMulti * modPlayer.bonusSpeedMultiplier;
            player.runAcceleration *= speedMulti * modPlayer.bonusSpeedMultiplier;
            if (player.jumpSpeedBoost < 1f)
            {
                player.jumpSpeedBoost = 1f;
            }
            player.jumpSpeedBoost *= speedMulti * modPlayer.bonusSpeedMultiplier;
            player.GetDamage(DamageClass.Generic) *= damageMulti;

            kiDrainMulti.SetValue(modPlayer, attackDrainMulti);
            KiDamage.SetValue(modPlayer, modPlayer.KiDamage * damageMulti);
        }
    }
}
