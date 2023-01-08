using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using DBZGoatLib.Handlers;
using DBTBalance.Model;

namespace DBTBalance.Helpers
{
   

    internal sealed class Hooks
    {
        public static void BaseBeam_ModifyHitNPC_Hook(
            dynamic self,
            NPC target,
            ref int damage,
            ref float knockback,
            ref bool crit,
            ref int hitDirection)
        {

            dynamic playerOwner = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("BaseBeam")).GetMethod("GetPlayerOwner").Invoke(self, null);
            damage = (int)(damage * ((float)playerOwner.kiDamage * 0.5f));
        }

        public delegate void orig_ResetEffects(dynamic self);
        public static void MyPlayer_ResetEffects_Hook(orig_ResetEffects orig,  dynamic self)
        {
            int regen = (int)self.kiRegen;
            orig(self);
            self.kiChargeRate += regen;
        }


        public static void MyPlayer_HandleTransformations_Hook(dynamic self)
        {
            if(!BalanceConfigServer.Instance.LongerTransform)
            {
                TransformPlayer(self, true);
                return;
            }

            int selection = 0;

            if (ModLoader.HasMod("dbzcalamity"))
            {
                selection = (int)DBTBalance.DBCA.Code.DefinedTypes.First(x => x.Name.Equals("TransMenu")).GetField("menuSelection").GetValue(null);
            }
            else
            {
                selection = (int)DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransMenu")).GetField("menuSelection").GetValue(null);
            }

            BPlayer player = BPlayer.ModPlayer(self.Player);

            if (TransformationHandler.TransformKey.Current && !TransformationHandler.IsTransformed(player.Player,false) && selection < 10 && selection > 0 )
            {
                self.isCharging = true;

                if (!player.PoweringUpTime.HasValue)
                {
                    player.PoweringUpTime = DateTime.Now;
                    player.LastPowerUpTick = DateTime.Now;

                    CombatText.NewText(player.Player.Hitbox, Color.Yellow, "3");
                    return;
                }

                else if (player.PoweringUpTime.HasValue && player.LastPowerUpTick.HasValue)
                {
                    int secs = (int)(3 - (DateTime.Now - player.PoweringUpTime.Value).TotalSeconds);
                    if ((DateTime.Now - player.LastPowerUpTick.Value).TotalMilliseconds >= 666 && secs > 0)
                    {
                        player.LastPowerUpTick = DateTime.Now;
                        CombatText.NewText(player.Player.Hitbox, Color.Yellow, $"{secs}");
                        return;
                    }
                    if ((DateTime.Now - player.PoweringUpTime.Value).TotalMilliseconds >= 2000)
                    {
                        TransformPlayer(self);
                    }
                }
            }
            else if (TransformationHandler.IsTransformed(player.Player,false) || selection > 10)
            {
                TransformPlayer(self, true);
            }
            else
            {
                player.PoweringUpTime = null;
                player.LastPowerUpTick = null;
            }
        }

        public static void TransformPlayer(dynamic self, bool justPress = false)
        {
            var TransformationHelper = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransformationHelper"));
            dynamic buff = null;
            if ((TransformationHandler.TransformKey.JustPressed && justPress) || (TransformationHandler.TransformKey.Current && !justPress))
            {
                if ((bool)TransformationHelper.GetMethod("IsPlayerTransformed").Invoke(null, new object[] { self.Player }))
                {
                    if (TransformationHandler.EnergyChargeKey.Current)
                    {
                        if (self.CanAscend())
                            buff = TransformationHelper.GetMethod("GetNextAscensionStep").Invoke(null, new object[] { self.Player });
                    }
                    else
                        buff = TransformationHelper.GetMethod("GetNextTransformationStep").Invoke(null, new object[] { self.Player });
                }
                else
                {
                    var selection = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransMenu")).GetField("menuSelection").GetValue(null);
                    buff = TransformationHelper.GetMethod("GetBuffFromMenuSelection").Invoke(null, new object[] { selection });
                }
            }
            else if (TransformationHandler.PowerDownKey.JustPressed && !(bool)TransformationHelper.GetMethod("IsKaioken", new Type[] { typeof(Player) }).Invoke(null, new object[] { self.Player }))
            {
                buff = TransformationHelper.GetMethod("GetPreviousTransformationStep").Invoke(null, new object[] { self.Player });
            }
            if (buff == null)
                return;

            var canTransform = TransformationHelper.GetMethod("CanTransform", new Type[] { typeof(Player), DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("BuffInfo")).AsType() });
            if (!(bool)canTransform.Invoke(null, new object[] { self.Player, buff }))
                return;

            TransformationHelper.GetMethod("DoTransform").Invoke(null, new object[] { self.Player, buff, DBTBalance.DBZMOD });
        }

        public delegate void orig_HandleChargingKi(dynamic self, Player player);

        public static void AbstractChargeBall_HandleChargingKi_Hook(orig_HandleChargingKi orig, dynamic self, Player player)
        {
            if (BalanceConfigServer.Instance.ChargeRework)
            {
                float limit = self.chargeLimit;
                int limitAdd = 0;
                if (player != null)
                {
                    dynamic modPlayer = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetMethod("ModPlayer").Invoke(null, new object[] { player });
                    limitAdd = modPlayer.chargeLimitAdd;
                }
                self.chargeRatePerSecond = (float)(limit + limitAdd) / 3f;
            }
            
            orig(self,player);
        }

        public static float BaseBeamCharge_GetBeamPowerMultiplier_Hook(dynamic self)
        {
            if (BalanceConfigServer.Instance.ChargeRework)
                return 1f + (float)self.ChargeLevel * 0.03f;
            else
                return 1f + (float)self.ChargeLevel / 20f;
        }

        public static int BaseBeamCharge_GetBeamDamage_Hook(dynamic self)
        {
            if (BalanceConfigServer.Instance.ChargeRework)
                return (int)Math.Ceiling((self.Projectile.damage * BaseBeamCharge_GetBeamPowerMultiplier_Hook(self))*0.7f);
            else
                return (int)Math.Ceiling(self.Projectile.damage * BaseBeamCharge_GetBeamPowerMultiplier_Hook(self));
        }
    }
}
