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
            if(BalanceConfigServer.Instance.KiRework)
                self.kiChargeRate += regen;
        }

        public delegate float orig_GetDodgeCost(dynamic self);

        public static float DBCA_GetDodgeCost_Hook(orig_GetDodgeCost orig, dynamic self)
        {
            float value = orig(self);

            if (TransformationHandler.IsTransformed((Player)self.Player))
                if (TransformationHandler.GetCurrentTransformation((Player)self.Player).Value.buffKeyName == "SSJRBuff")
                {
                    var type = DBTBalance.DBCA.Code.DefinedTypes.First(x => x.Name.Equals("dbzcalamityPlayer"));
                    var isAngelic = type.GetField("IsAngelic");
                    return 300 * ((bool)isAngelic.GetValue(self) ? 0.66f : 1f);
                }

            return value;
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

        public static float MyPlayer_PowerWishMulti_Hook(dynamic self)
        {
            return (float)(1f + (0.05 * (int)self.GetPowerWishesUsed()));
        }
        public static void MyPlayer_HandlePowerWishMultipliers_Hook(dynamic self)
        {
            float multi = MyPlayer_PowerWishMulti_Hook(self);
            Player player = (Player)self.Player;

            player.GetDamage(DamageClass.Generic) *= multi;
            self.KiDamage *= multi;
        }
    }
}
