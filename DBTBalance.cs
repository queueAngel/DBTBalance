using Terraria.ModLoader;
using System;
using System.Reflection;
using Terraria;
using DBTBalance.Helpers;
using System.IO;
using DBZGoatLib;
using dbzcalamity;
using DBZMODPORT.Projectiles;
using DBZMODPORT;

namespace DBTBalance
{
	public class DBTBalance : Mod
	{
		public static DBTBalance Instance;
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
            //MonoModHooks.Add(typeof(TransBuff).GetMethod("AssembleTransBuffDescription"), BuffHooks.BuildTooltip_Hook); // look into this properly later

            // Replaced with MonoModHooks usage above - qAngel

            /*
            var BaseProj = DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("BaseBeam"));
            var myPlayer = DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
            var baseBeamCharge = DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("BaseBeamCharge"));
            var AbstractChargeBall = DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("AbstractChargeBall"));
            var transBuff = DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransBuff"));

            AddHook(BaseProj.AsType(), "ModifyHitNPC", typeof(Hooks), "BaseBeam_ModifyHitNPC_Hook");

            AddHook(myPlayer.AsType(), "ResetEffects", typeof(Hooks), "MyPlayer_ResetEffects_Hook");

            AddHook(myPlayer.AsType(), "PowerWishMulti", typeof(Hooks), "MyPlayer_PowerWishMulti_Hook");

            AddHook(myPlayer.AsType(), "HandlePowerWishMultipliers", typeof(Hooks), "MyPlayer_HandlePowerWishMultipliers_Hook");

            AddHook(baseBeamCharge.AsType(), "GetBeamPowerMultiplier", typeof(Hooks), "BaseBeamCharge_GetBeamPowerMultiplier_Hook");
            AddHook(baseBeamCharge.AsType(), "GetBeamDamage", typeof(Hooks), "BaseBeamCharge_GetBeamDamage_Hook");

            AddHook(AbstractChargeBall.AsType(), "HandleChargingKi", typeof(Hooks), "AbstractChargeBall_HandleChargingKi_Hook");

            AddHook(transBuff.AsType(), "AssembleTransBuffDescription", typeof(BuffHooks), "BuildTooltip_Hook");
            */

            foreach (var type in AccessoryHooks.UpgradeTypes)
            {
                MonoModHooks.Add(type.GetMethod("UpdateAccessory"), AccessoryHooks.Update_hook);
            }
            // Replaced with localization file - qAngel

            /*
            foreach (var acc in AccessoryHooks.ChagnedTooltips)
            {
                var type = DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals(acc.Key));
                MonoModHooks.Add(type.AsType().GetMethod("SetStaticDefaults", flagsAll), AccessoryHooks.SetStaticDefaults_Hook);
            }
            */

            foreach (var adj in BuffHooks.Adjustments)
            {
                MonoModHooks.Add(adj.Key.GetMethod("SetStaticDefaults"), BuffHooks.SetStaticDefaults_Hook);
                MonoModHooks.Add(adj.Key.GetMethod("Update", [typeof(Player), typeof(int).MakeByRefType()]), BuffHooks.Update_Hook);
            }

            // Replaced with MonoModHooks usage above - qAngel

            /*
            foreach (var adj in BuffHooks.DBT_Adjustments)
            {
                var type = DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals(adj.Key));

                AddHook(type.AsType(), "SetStaticDefaults", typeof(BuffHooks), "SetStaticDefaults_Hook");
                AddHook(type.AsType(), "Update", new Type[] { typeof(Player), typeof(int).MakeByRefType() }, typeof(BuffHooks), "Update_Hook");
            }
            */

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

            // Should be handled by AccessoryHooks.UpgradeTypes previously - qAngel

            /*
            foreach (var acc in AccessoryHooks.dbcaUpgradePaths)
            {
                var type = dbca.Code.DefinedTypes.First(x => x.Name.Equals(acc.Key));

                AddHook(type.AsType(), "UpdateAccessory", typeof(AccessoryHooks), "Update_hook");
            }
            */

            // Replaced with localization file - qAngel

            /*
            foreach (var acc in AccessoryHooks.DBCAChagnedToolttips)
            {
                var type = dbca.Code.DefinedTypes.First(x => x.Name.Equals(acc.Key));

                AddHook(type.AsType(), "SetStaticDefaults", typeof(AccessoryHooks), "SetStaticDefaults_Hook");
            }
            */

            // Should be handled by BuffHooks.Adjustments and ArmorHooks.ArmorTypes previously - qAngel

            /*
            foreach (var adj in BuffHooks.DBCA_Adjustments)
            {
                var type = dbca.Code.DefinedTypes.First(x => x.Name.Equals(adj.Key));

                AddHook(type.AsType(), "SetStaticDefaults", typeof(BuffHooks), "SetStaticDefaults_Hook");
            }
            foreach (var armor in ArmorHooks.DBCA_Armor)
            {
                var type = dbca.Code.DefinedTypes.First(x => x.Name.Equals(armor));
                AddHook(type.AsType(), "SetDefaults", typeof(ArmorHooks), "Armor_SetStaticDefaults");
            }
            */
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