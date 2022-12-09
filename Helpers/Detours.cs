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
    internal sealed class Detours
    {

        public static Dictionary<string, (float Damage, float Speed, int defense)> DBT_Adjustments = new()
        {
            { "SSJ1Buff", (1.20f, 1.2f, 4) },
            { "ASSJBuff", (1.25f, 1.2f, 5) },
            { "USSJBuff", (1.28f, 0.9f, 7) },
            { "SuperKaiokenBuff", (1.29f, 1.2f, 7) },
            { "SSJ2Buff", (1.3f, 1.24f, 8) },
            { "SSJ3Buff", (1.4f, 1.3f, 12) },
            { "SSJGBuff", (1.6f, 1.35f, 16) },
            { "SSJBBuff", (1.75f, 1.55f, 20) },
            { "SSJRBuff", (1.85f, 1.4f, 17) },
            { "LSSJBuff", (1.35f, 1.3f, 6) },
            { "LSSJ2Buff", (1.5f, 1.35f, 14) },
            { "LSSJ3Buff", (1.65f, 1.4f, 20) }
        };

        public static Dictionary<string, (float Damage, float Speed, int defense, float dodgeBonus, float minDamage, float maxDamage)> DBCA_Adjustments = new()
        {
            { "UISignBuff", (1.9f, 1.4f, 20, 0.2f, 0f, 0f) },
            { "UIBuff", (2f, 1.55f, 22, 0.2f, 0f,0f) },
            { "UEBuff", (2.2f, 1.3f, 18, 0f, 2f, 2.5f) }
        };

        public static Dictionary<string, dynamic> cachedBuffs = new();
        public static Dictionary<int, string> cachedTypes = new();

        public static dynamic GetExternalForm(Player player)
        {
            foreach (var type in cachedTypes)
            {
                if (player.HasBuff(type.Key))
                    return cachedBuffs[type.Value];
            }
            return null;
        }

        public void UI_Update_Detour(Player player, ref int buffIndex)
        {
            dynamic buff = GetExternalForm(player);

            if (buff == null) return;

            if (ModLoader.HasMod("dbzcalamity"))
            {
                var dbcPlayerClass = DBTBalance.DBCA.Code.DefinedTypes.First(x => x.Name.Equals("dbzcalamityPlayer"));
                dynamic dbcPlayer = dbcPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { player });
                bool IsAngelic = (bool)dbcPlayerClass.GetMethod("IsPlayerAngelic").Invoke(dbcPlayer, null);
                var stopAuraSound = dbcPlayerClass.GetMethod("StopAuraSound");
                var puiActive = dbcPlayerClass.GetField("puiActive");
                var uiActive = dbcPlayerClass.GetField("uiActive");

                dynamic modPlayer = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetMethod("ModPlayer").Invoke(null, new object[] { player });
                var KiDamage = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetField("KiDamage");
                var kiDrainMulti = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetField("kiDrainMulti");


                Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(Utils.NextFloat(Main.rand, (float)(-(float)player.width - 2), (float)(player.width + 2)), Utils.NextFloat(Main.rand, (float)(-(float)player.height - 6), (float)(player.height + 6))), 221, new Vector2?(Utils.RotatedByRandom(new Vector2(0f, -1.5f), (double)MathHelper.ToRadians(80f))), 0, default(Color), 1f);
                Lighting.AddLight(player.Center, Color.Blue.ToVector3());
                dust.noGravity = true;
                if (player.HasBuff(cachedBuffs["UIBuff"].Type))
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
                float speedMulti = buff.speedMulti;
                float damageMulti = buff.damageMulti;
                float attackDrainMulti = buff.attackDrainMulti;
                int baseDefenceBonus = buff.baseDefenceBonus;
                float kiDrainRate = buff.kiDrainRate;
                
                if (cachedTypes[buff.Type] == "UEBuff")
                {
                    float maxDamage = buff.maxDamage ?? 0f;
                    float minDamage = buff.minDamage ?? 0f;

                    var ueDamageBonusPerDamageTaken = dbcPlayerClass.GetField("ueDamageBonusPerDamageTaken");

                    ueDamageBonusPerDamageTaken.SetValue(dbcPlayer, buff.damageBonusPerDamageTaken ?? 0f);

                    buff.baseMaxDamage = maxDamage;

                    if ((bool)dbcPlayerClass.GetField("destructionUEboostBuff").GetValue(dbcPlayer))
                    {
                        maxDamage += 0.1f;
                        buff.ueDamageBonusPerDamageTaken = buff.baseBonusPerDamage + buff.baseBonusPerDamage / 2f;
                    }
                    else
                    {
                        buff.maxDamage = buff.baseMaxDamage;

                    }

                    buff.damageBonusPerDamageTaken = buff.baseBonusPerDamage;

                    damageMulti = Math.Min(buff.maxDamage, buff.minDamage + (float)dbcPlayerClass.GetField("ueCurrentDamageBonus").GetValue(dbcPlayer));
                }
                else
                {
                    float dodgeChance = buff.dodgeChance ?? 0f;
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
                    if (player.HasBuff(cachedBuffs["UISignBuff"].Type))
                        modPlayer.AddKi(cachedBuffs["UISignBuff"].kiDrainRate * -1f, false, true);

                    if (player.HasBuff(cachedBuffs["UIBuff"].Type))
                        modPlayer.AddKi(cachedBuffs["UIBuff"].kiDrainRate * -1f, false, true);
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
                player.GetDamage(DamageClass.Generic) += damageMulti -1f;

                kiDrainMulti.SetValue(modPlayer, attackDrainMulti);
                KiDamage.SetValue(modPlayer, modPlayer.KiDamage * damageMulti);
            }
        }
        
    }
}
