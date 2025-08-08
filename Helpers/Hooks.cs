using System;
using Terraria;
using Terraria.ModLoader;
using DBZGoatLib.Handlers;
using DBTBalanceRevived.Model;
using DBZMODPORT.Projectiles;
using DBZMODPORT;
using static DBTBalanceRevived.Helpers.Hooks;
using dbzcalamity;
using MonoMod.Cil;
using Terraria.ID;

namespace DBTBalanceRevived.Helpers
{
    internal static class Hooks
    {
        public delegate void orig_ModifyHitNPC(ModProjectile self, NPC target, ref NPC.HitModifiers modifiers);
        public static void BaseBeam_ModifyHitNPC_Hook(orig_ModifyHitNPC orig, ModProjectile self, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (self is not BaseBeam beam)
            {
                orig(self, target, ref modifiers);
                return;
            }
            MyPlayer playerOwner = beam.GetPlayerOwner();
            // eh, here it originally used kiDamage which is a float. not sure what the equivalent would be with the new damageclass system
            // lol now i went back to an older version
            modifiers.SourceDamage += playerOwner.kiDamage * 0.5f; // playerOwner.Player.GetDamage<KiDamageType>().Flat * 0.5f;
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

        public delegate void orig_HandleChargingKi(AbstractChargeBall self, Player player);

        public static void AbstractChargeBall_HandleChargingKi_Hook(orig_HandleChargingKi orig, dynamic self, Player player)
        {
            if (BalanceConfigServer.Instance.ChargeRework)
            {
                float limit = self.chargeLimit;
                int limitAdd = 0;
                if (player != null)
                {
                    MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
                    limitAdd = modPlayer.chargeLimitAdd;
                }
                self.chargeRatePerSecond = (float)(limit + limitAdd) / 3f;
            }
            
            orig(self,player);
        }

        public static float BaseBeamCharge_GetBeamPowerMultiplier_Hook(BaseBeamCharge self)
        {
            if (BalanceConfigServer.Instance.ChargeRework)
                return 1f + (float)self.ChargeLevel * 0.03f;
            else
                return 1f + (float)self.ChargeLevel / 20f;
        }

        public static int BaseBeamCharge_GetBeamDamage_Hook(BaseBeamCharge self)
        {
            if (BalanceConfigServer.Instance.ChargeRework)
                return (int)Math.Ceiling((self.Projectile.damage * BaseBeamCharge_GetBeamPowerMultiplier_Hook(self))*0.7f);
            else
                return (int)Math.Ceiling(self.Projectile.damage * BaseBeamCharge_GetBeamPowerMultiplier_Hook(self));
        }

        public static float MyPlayer_PowerWishMulti_Hook(MyPlayer self)
        {
            return (float)(0.05 * self.GetPowerWishesUsed());
        }
        public static void MyPlayer_HandlePowerWishMultipliers_Hook(MyPlayer self)
        {
            float multi = MyPlayer_PowerWishMulti_Hook(self);
            self.Player.GetDamage(DamageClass.Generic) += multi;
            //self.Player.GetDamage<KiDamageType>() += multi;
            self.kiDamage += multi;
        }
        public static void KiProjectile_OnHitNPC_Hook(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(i => i.MatchLdcI4(BuffID.OnFire));
            c.Next.Operand = BuffID.Bleeding;
        }
    }
    [JITWhenModsEnabled("dbzcalamity")]
    internal static class DBCAHooks
    {
        public static float DBCA_GetDodgeCost_Hook(orig_GetDodgeCost orig, dbzcalamityPlayer self)
        {
            float value = orig(self);

            if (TransformationHandler.IsTransformed(self.Player))
                if (TransformationHandler.GetCurrentTransformation(self.Player)?.buffKeyName == "SSJRBuff")
                    return 300 * (self.IsAngelic ? 0.66f : 1f);

            return value;
        }
    }
}
