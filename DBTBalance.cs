using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria;
using System.Dynamic;
using DBTBalance.Buffs;
using DBTBalance.Helpers;
using System.IO;
using DBZGoatLib;
using DBTBalance.Model;
using Terraria.ModLoader.Config;

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

        internal List<Detour> Detours = new List<Detour>();

        public override void Load()
        {
            Instance = this;

            if(ModLoader.TryGetMod("DBZGoatLib", out Mod goat))
            {
                GOATLIB = goat;
                DBZGoatLib.Handlers.TransformationHandler.RegisterTransformation(LSSJ4Buff.LSSJ4Info);
            }
            if (ModLoader.TryGetMod("DBZMODPORT", out Mod dbz))
            {
                DBZMOD = dbz;
            }
            
            if(ModLoader.TryGetMod("dbzcalamity",out Mod dbca))
            {
                DBCA = dbca;

                MonoModHooks.RequestNativeAccess();

                var MUI = DBCA.Code.DefinedTypes.First(x => x.Name.Equals("UIBuff"));
                var UI = DBCA.Code.DefinedTypes.First(x => x.Name.Equals("UISignBuff"));
                var UE = DBCA.Code.DefinedTypes.First(x => x.Name.Equals("UEBuff"));

                AddDetour(MUI.AsType(), "Update", new Type[] { typeof(Player), typeof(int).MakeByRefType() }, typeof(Detours), "UI_Update_Detour");
                AddDetour(UI.AsType(), "Update", new Type[] { typeof(Player), typeof(int).MakeByRefType() }, typeof(Detours), "UI_Update_Detour");
                AddDetour(UE.AsType(), "Update", new Type[] { typeof(Player), typeof(int).MakeByRefType() }, typeof(Detours), "UI_Update_Detour");
            }

        }
        public override void Unload()
        {
            Instance = null;
            DBZMOD = null;
            DBCA = null;
            GOATLIB = null;

            DBZGoatLib.Handlers.TransformationHandler.UnregisterTransformation(LSSJ4Buff.LSSJ4Info);

            for (int i = 0; i < Detours.Count; i++)
                if (Detours[i].IsApplied)
                    Detours[i].Dispose();
        }

        internal static void SaveConfig(BalanceConfig cfg)
        {
            MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
            if (saveMethodInfo != null)
            {
                saveMethodInfo.Invoke(null, new object[] { cfg });
                return;
            }
            Instance.Logger.Warn("In-game SaveConfig failed, code update required");
        }
        
        public void AddDetour(Type type, string name, bool methodType, Type to, string toName)
        {
            Logger.Info($"type {type.FullName}   name {name}   methodType {(methodType ? "Method" : "Property")}   to {to.FullName}   toName {toName}");

            MethodInfo method;
            if (methodType)
            {
                method = type.GetMethod(name, flagsAll);
            }
            else
            {
                method = type.GetProperty(name, flagsAll).GetMethod;
            }

            var detour = new Detour(method, to.GetMethod(toName, flagsAll));

            detour.Apply();

            Detours.Add(detour);
        }

        public void AddDetour(Type type, string name, Type[] args, Type to, string toName)
        {
            Logger.Info($"type {type.FullName}   name {name}   args {{{string.Join(",", args.Select(a => a.FullName))}}}   to {to.FullName}   toName {toName}");

            Detours.Add(new Detour(type.GetMethod(name, args), to.GetMethod(toName, flagsAll)));
        }

        public void AddDetour(Type type, string name) =>
            Detours.Add(new Detour(type.GetMethod(name, flagsAll), GetType().GetMethod("Nothing", flagsAll)));

        public static bool HasField(dynamic obj, string name)
        {
            Type objType = obj.GetType();

            if (objType == typeof(ExpandoObject))
            {
                return ((IDictionary<string, object>)obj).ContainsKey(name);
            }

            return objType.BaseType.GetField(name) != null;
        }
    }
}