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
using DBTBalance.Model;
using DBZGoatLib.Model;

namespace DBTBalance
{
    public class BPlayer : ModPlayer
    {
        public bool LSSJ4Achieved;
        public bool LSSJ4Active;
        public bool LSSJ4UnlockMsg;
        public bool MP_Unlock;

        public DateTime? Offset = null;

        public DateTime? PoweringUpTime = null;
        public DateTime? LastPowerUpTick = null;

        private TransformationInfo? Form = null;

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

                if (BalanceConfigServer.Instance.KiRework)
                {
                    var MyPlayerClass = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
                    dynamic myPlayer = MyPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { Player });
                    var kiRegenTimer = MyPlayerClass.GetField("kiRegenTimer", DBTBalance.flagsAll);
                    var kiChargeRate = MyPlayerClass.GetField("kiChargeRate");

                    kiRegenTimer.SetValue(myPlayer, 0);
                    kiChargeRate.SetValue(myPlayer, myPlayer.kiChargeRate + myPlayer.kiRegen);
                }
            }
        }

        public override void PostUpdate()
        {
            if (MP_Unlock && Main.netMode == NetmodeID.MultiplayerClient)
            {
                dynamic modPlayer = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetMethod("ModPlayer").Invoke(null, new object[] { Player });

                float mastery = (float)modPlayer.masteryLevelLeg3;

                if (mastery >= 1f)
                {
                    LSSJ4Achieved = true;
                    MP_Unlock = false;
                }
            }
            if(LSSJ4Achieved && !LSSJ4UnlockMsg)
            {
                LSSJ4UnlockMsg = true;
                if(Main.netMode != NetmodeID.Server)
                    Main.NewText("You have unlocked your true potential.\nWhile in Legendary Super Saiyain 3 form press the Transform button once more to reach higher power.", Color.Green);
            }

            if (ModLoader.HasMod("DBZMODPORT"))
            {
                if (BalanceConfigServer.Instance.KiRework)
                {
                    var MyPlayerClass = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
                    dynamic myPlayer = MyPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { Player });
                    var kiChargeRate = MyPlayerClass.GetField("kiChargeRate");

                    kiChargeRate.SetValue(myPlayer, myPlayer.kiChargeRate + myPlayer.kiRegen);
                }
            }
        }
        public override void ResetEffects()
        {
            if (ModLoader.HasMod("DBZMODPORT"))
            {
                if (BalanceConfigServer.Instance.KiRework)
                {
                    var MyPlayerClass = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
                    dynamic myPlayer = MyPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { Player });
                    var kiChargeRate = MyPlayerClass.GetField("kiChargeRate");

                    kiChargeRate.SetValue(myPlayer, myPlayer.kiChargeRate + myPlayer.kiRegen);
                }
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if(!Form.HasValue || TransformationHandler.TransformKey.JustPressed)
                Form = Player.GetModPlayer<GPlayer>().FetchTransformation();
            else if (Form.HasValue && !TransformationHandler.TransformKey.Current)
                Form = Player.GetModPlayer<GPlayer>().FetchTransformation();


            if (!Form.HasValue && TransformationHandler.PowerDownKey.JustPressed)
                TransformationHandler.ClearTransformations(Player);

            if (!BalanceConfigServer.Instance.LongerTransform)
            {
                if (Form.HasValue)
                    TransformationHandler.Transform(Player, Form.Value);
            }
            else
            {
                HandleTransformationInput();
            }
        }

        public void HandleTransformationInput()
        {
            dynamic self = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetMethod("ModPlayer").Invoke(null, new object[] { Player });
            if (TransformationHandler.TransformKey.Current && !TransformationHandler.IsTransformed(Player))
            {
                self.isCharging = true;

                if (!PoweringUpTime.HasValue)
                {
                    PoweringUpTime = DateTime.Now;
                    LastPowerUpTick = DateTime.Now;

                    CombatText.NewText(Player.Hitbox, Color.Yellow, "3");
                    return;
                }

                else if (PoweringUpTime.HasValue && LastPowerUpTick.HasValue)
                {
                    int secs = (int)(3 - (DateTime.Now - PoweringUpTime.Value).TotalSeconds);
                    if ((DateTime.Now - LastPowerUpTick.Value).TotalMilliseconds >= 666 && secs > 0)
                    {
                        LastPowerUpTick = DateTime.Now;
                        CombatText.NewText(Player.Hitbox, Color.Yellow, $"{secs}");
                        return;
                    }
                    if ((DateTime.Now - PoweringUpTime.Value).TotalMilliseconds >= 2000)
                    {
                        if (Form.HasValue)
                            TransformationHandler.Transform(Player, Form.Value);
                    }
                }
            }
            else if (TransformationHandler.IsTransformed(Player))
            {
                if (Form.HasValue)
                    TransformationHandler.Transform(Player, Form.Value);
            }
            else
            {
                PoweringUpTime = null;
                LastPowerUpTick = null;
            }
        }
    }
}
