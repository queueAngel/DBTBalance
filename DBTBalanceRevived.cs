using Terraria.ModLoader;
using System;
using System.Reflection;
using Terraria;
using DBTBalanceRevived.Helpers;
using System.IO;
using DBZGoatLib;
using dbzcalamity;
using DBZMODPORT.Projectiles;
using DBZMODPORT;

namespace DBTBalanceRevived
{
	public class DBTBalanceRevived : Mod
	{
		public static DBTBalanceRevived Instance;
        public static Mod DBZMOD;
        public static Mod DBCA;
        public static Mod GOATLIB;

        public const BindingFlags flagsAll = BindingFlags.Public | BindingFlags.NonPublic
        | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetField
        | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;
        public override void PostSetupContent()
        {
            AccessoryHooks.Initialize();
        }
        public override void Load()
        {
            Instance = this;

            ModLoader.TryGetMod("DBZMODPORT", out DBZMOD);

            Type myPlayer = typeof(MyPlayer);
            Type baseBeamCharge = typeof(BaseBeamCharge);

            MonoModHooks.Add(typeof(BaseBeam).GetMethod("ModifyHitNPC"), Hooks.BaseBeam_ModifyHitNPC_Hook);
            MonoModHooks.Add(myPlayer.GetMethod("ResetEffects"), Hooks.MyPlayer_ResetEffects_Hook);
            MonoModHooks.Add(myPlayer.GetMethod("PowerWishMulti"), Hooks.MyPlayer_PowerWishMulti_Hook);
            MonoModHooks.Add(myPlayer.GetMethod("HandlePowerWishMultipliers"), Hooks.MyPlayer_HandlePowerWishMultipliers_Hook);
            MonoModHooks.Add(baseBeamCharge.GetMethod("GetBeamPowerMultiplier", BindingFlags.Instance | BindingFlags.NonPublic), Hooks.BaseBeamCharge_GetBeamPowerMultiplier_Hook);
            MonoModHooks.Add(baseBeamCharge.GetMethod("GetBeamDamage", BindingFlags.Instance | BindingFlags.NonPublic), Hooks.BaseBeamCharge_GetBeamDamage_Hook);
            MonoModHooks.Add(typeof(AbstractChargeBall).GetMethod("HandleChargingKi"), Hooks.AbstractChargeBall_HandleChargingKi_Hook);

            foreach (var type in AccessoryHooks.UpgradeTypes)
            {
                MonoModHooks.Add(type.GetMethod("UpdateAccessory"), AccessoryHooks.Update_hook);
            }

            foreach (var adj in BuffHooks.Adjustments)
            {
                MonoModHooks.Add(adj.Key.GetMethod("SetStaticDefaults"), BuffHooks.SetStaticDefaults_Hook);
                MonoModHooks.Add(adj.Key.GetMethod("Update", [typeof(Player), typeof(int).MakeByRefType()]), BuffHooks.Update_Hook);
            }

            foreach (var armor in ArmorHooks.ArmorTypes)
            {
                MonoModHooks.Add(armor.GetMethod("SetDefaults"), ArmorHooks.Armor_SetStaticDefaults); // idk why this hook is named wrong but i'm gonna keep it :)
            }

            if (ModLoader.TryGetMod("DBZGoatLib", out Mod goat))
            {
                GOATLIB = goat;

                MonoModHooks.Add(typeof(GPlayer).GetMethod("ProcessTransformationTriggers"), (Action<GPlayer> orig, GPlayer self) => { });
            }
            
            if(ModLoader.TryGetMod("dbzcalamity", out Mod dbca))
            {
                DBCA = dbca;

                LoadHooksCalamity();
            }

        }

        [JITWhenModsEnabled("dbzcalamity")]
        private static void LoadHooksCalamity()
        {
            MonoModHooks.Add(typeof(dbzcalamityPlayer).GetMethod("GetDodgeCost"), DBCAHooks.DBCA_GetDodgeCost_Hook);
        }
        public override void Unload()
        {
            Instance = null;
            DBZMOD = null;
            DBCA = null;
            GOATLIB = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            BNetworkHandler.HandlePacket(reader, whoAmI);
        }
    }
}