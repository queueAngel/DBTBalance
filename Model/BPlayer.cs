using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using DBTBalance.Buffs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameInput;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using DBTBalance.Helpers;
using DBZGoatLib.Handlers;
using DBZGoatLib;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace DBTBalance
{
    public class BPlayer : ModPlayer
    {
        public bool LSSJ4Achieved;
        public bool LSSJ4Active;
        public bool LSSJ4UnlockMsg;

        public DateTime? Offset = null;

        public override void SaveData(TagCompound tag)
        {
            tag.Add("DBTBalance_LSSJ4Achieved", LSSJ4Achieved);
            tag.Add("DBTBalance_LSSJ4UnlockMsg", LSSJ4UnlockMsg);
        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("DBTBalance_LSSJ4Achieved"))
                LSSJ4Achieved = tag.GetBool("DBTBalance_LSSJ4Achieved");
            if (tag.ContainsKey("DBTBalance_LSSJ4UnlockMsg"))
                LSSJ4UnlockMsg = tag.GetBool("DBTBalance_LSSJ4UnlockMsg");
        }
        
        public static BPlayer ModPlayer(Player player) => player.GetModPlayer<BPlayer>();
       
        public static bool MasteredLLSJ4(Player player) => GPlayer.ModPlayer(player).GetMastery(ModContent.BuffType<LSSJ4Buff>()) >= 1f;
        
        public override void PreUpdateBuffs()
        {
            if (ModLoader.HasMod("DBZMODPORT"))
            {
                foreach(var adjustment in Detours.DBT_Adjustments)
                {
                    if (!Detours.cachedBuffs.ContainsKey(adjustment.Key))
                        Detours.cachedBuffs[adjustment.Key] = DBTBalance.DBZMOD.Find<ModBuff>(adjustment.Key);
                    if (!Detours.cachedTypes.ContainsKey(Detours.cachedBuffs[adjustment.Key].Type))
                        Detours.cachedTypes[Detours.cachedBuffs[adjustment.Key].Type] = adjustment.Key;
                    Detours.cachedBuffs[adjustment.Key].damageMulti = adjustment.Value.Damage;
                    Detours.cachedBuffs[adjustment.Key].speedMulti = adjustment.Value.Speed;
                    Detours.cachedBuffs[adjustment.Key].baseDefenceBonus = adjustment.Value.defense;
                }
            }
            if(ModLoader.HasMod("dbzcalamity"))
            {
                foreach(var adjustment in Detours.DBCA_Adjustments)
                {
                    if (!Detours.cachedBuffs.ContainsKey(adjustment.Key))
                        Detours.cachedBuffs[adjustment.Key] = DBTBalance.DBCA.Find<ModBuff>(adjustment.Key);
                    if (!Detours.cachedTypes.ContainsKey(Detours.cachedBuffs[adjustment.Key].Type))
                        Detours.cachedTypes[Detours.cachedBuffs[adjustment.Key].Type] = adjustment.Key;

                    Detours.cachedBuffs[adjustment.Key].damageMulti = adjustment.Value.Damage;
                    Detours.cachedBuffs[adjustment.Key].speedMulti = adjustment.Value.Speed;
                    Detours.cachedBuffs[adjustment.Key].baseDefenceBonus = adjustment.Value.defense;

                    if (adjustment.Value.dodgeBonus > 0f)
                        Detours.cachedBuffs[adjustment.Key].dodgeChance = adjustment.Value.dodgeBonus;
                    if(adjustment.Value.minDamage > 0f)
                        Detours.cachedBuffs[adjustment.Key].minDamage = adjustment.Value.minDamage;
                    if (adjustment.Value.maxDamage > 0f)
                        Detours.cachedBuffs[adjustment.Key].maxDamage = adjustment.Value.maxDamage;

                }
            }
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if(ModLoader.TryGetMod("dbzcalamity", out Mod dbcamod))
            {
                var ModPlayerClass = dbcamod.Code.DefinedTypes.First(x => x.Name.Equals("dbzcalamityPlayer"));
                var getModPlayer = ModPlayerClass.GetMethod("ModPlayer");

                dynamic dbzPlayer = getModPlayer.Invoke(null, new object[] {Player});

                var dodgeChance = ModPlayerClass.GetField("dodgeChange");
                dodgeChance.SetValue(dbzPlayer, (float)dodgeChance.GetValue(dbzPlayer) - .05f);
            }

        }

        public override void PreUpdate()
        {
            if (ModLoader.HasMod("DBZMODPORT"))
            {
                var transformationHelper = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransformationHelper"));
                bool IsLSSJ3 = (bool)transformationHelper.GetMethod("IsLSSJ3").Invoke(null, new object[] { Player });

                if (!IsLSSJ3)
                    Offset = null;

                if (IsLSSJ3 && !Offset.HasValue)
                    Offset = DateTime.Now;
            }
        }

        public override void PostUpdate()
        {
            if(LSSJ4Achieved && !LSSJ4UnlockMsg)
            {
                LSSJ4UnlockMsg = true;
                Main.NewText("You have unlocked your true potential.\nWhile in Legendary Super Saiyain 3 form press the Transform button once more to reach higher power.", Color.Green);
            }
            var buff = Detours.GetExternalForm(Player);
            

            if (buff != null)
            {
                string name = Detours.cachedTypes[buff.Type];
                if (name != "UIBuff" && name != "UEBuff" && name != "UISignBuff")
                {
                    float dmg = (float)(1.0 + ((double)(buff.damageMulti) - 1.0) * 0.5);

                    Player.GetDamage(DamageClass.Generic) *= dmg;
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            var modPlayerClass = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
            var modPlayer = modPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { Player });

            if (TransformationHandler.TransformKey.JustPressed && !LSSJ4Active && Offset.HasValue && TransformationHandler.IsTransformed(Player, false))
            {
                var transformationHelper = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransformationHelper"));
                bool IsLSSJ3 = (bool)transformationHelper.GetMethod("IsLSSJ3").Invoke(null, new object[] { Player });

                if (IsLSSJ3 && (DateTime.Now - Offset.Value).TotalSeconds >= 0.8f)
                {
                    TransformationHandler.Transform(Player, LSSJ4Buff.LSSJ4Info);
                    Offset = null;
                }
            }
            else if (TransformationHandler.PowerDownKey.JustPressed && LSSJ4Active && !TransformationHandler.EnergyChargeKey.Current)
            {
                Offset = null;
                TransformationHandler.ClearTransformations(Player);
            }
            else if (TransformationHandler.EnergyChargeKey.Current && TransformationHandler.PowerDownKey.JustPressed && LSSJ4Active)
            {
                Offset = null;
                TransformationHandler.ClearTransformations(Player);
                var THandler = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransformationHelper"));

                var doTransform = THandler.GetMethod("DoTransform");

                var lssj3 = THandler.GetProperty("LSSJ3").GetValue(null);

                doTransform.Invoke(null, new object[] { Player, lssj3, DBTBalance.DBZMOD });
            }
        }

        public override void OnRespawn(Player player)
        {
            TransformationHandler.ClearTransformations(player);
            base.OnRespawn(player);
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (LSSJ4Active)
            {
                
                drawInfo.colorArmorBody = new Color(0,0,0,0);
                drawInfo.colorShirt = new Color(0,0,0,0);
            }
        }
    }
}
