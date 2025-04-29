using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using DBTBalanceRevived.Buffs;
using Terraria.GameInput;
using Microsoft.Xna.Framework;
using DBZGoatLib.Handlers;
using DBZGoatLib;
using DBTBalanceRevived.Model;
using DBZGoatLib.Model;
using DBZMODPORT;
using dbzcalamity;
using DBZMODPORT.Util;
using System.Runtime.CompilerServices;
using Terraria.Localization;

namespace DBTBalanceRevived
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
        /// <summary>
        /// i will die before using a list as a buffer
        /// </summary>
        public ushort[] upgradePathBuffer;
        public override void SaveData(TagCompound tag)
        {
            if (LSSJ4Achieved)
                tag.Add("DBTBalanceRevived_LSSJ4Achieved", LSSJ4Achieved);
            if (LSSJ4UnlockMsg)
                tag.Add("DBTBalanceRevived_LSSJ4UnlockMsg", LSSJ4UnlockMsg);
        }
        public override void LoadData(TagCompound tag)
        {
            LSSJ4Achieved = tag.ContainsKey("DBTBalanceRevived_LSSJ4Achieved");
            LSSJ4UnlockMsg = tag.ContainsKey("DBTBalanceRevived_LSSJ4UnlockMsg");
        }
        
        public static BPlayer ModPlayer(Player player) => player.GetModPlayer<BPlayer>();
        public static bool MasteredLLSJ4(Player player) => GPlayer.ModPlayer(player).GetMastery(ModContent.BuffType<LSSJ4Buff>()) >= 1f;
        public void AddToUpgradesBuffer(int type)
        {
            upgradePathBuffer ??= new ushort[8]; // let's say 8

            // check if the player already has the accessory. also for a free slot. this is safe because accessories will always be added to the buffer in order
            for (int i = 0; i < upgradePathBuffer.Length; i++)
            {
                ref ushort slot = ref upgradePathBuffer[i];
                if (slot == type)
                {
                    return; // exit method if they do
                }
                else if (slot == ItemID.None)
                {
                    slot = (ushort)type; // store accessory if empty slot is found
                    return;
                }
            }

            // if we're still in the method, that means the array didn't have enough slots, so resize and add it to the new slot
            Array.Resize(ref upgradePathBuffer, upgradePathBuffer.Length + 1);
            upgradePathBuffer[^1] = (ushort)type;
        }
        public override void PostUpdateEquips()
        {
            if (ModLoader.HasMod("dbzcalamity"))
                PostUpdateEquipsCalamity();
        }
        [JITWhenModsEnabled("dbzcalamity")]
        internal void PostUpdateEquipsCalamity()
        {
            dbzcalamityPlayer modPlayer = Player.GetModPlayer<dbzcalamityPlayer>();
            modPlayer.dodgeChange -= 0.05f;
        }
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "kiRegenTimer")]
        public static extern ref int GetKiRegenTimer(MyPlayer instance);
        public override void PreUpdate()
        {
            bool isLSSJ3 = TransformationHelper.IsLSSJ3(Player);

            if (!isLSSJ3)
                Offset = null;
            else if (!Offset.HasValue)
                Offset = DateTime.Now;

            if (BalanceConfigServer.Instance.KiRework)
            {
                MyPlayer modPlayer = Player.GetModPlayer<MyPlayer>();
                ref int kiRegenTimer = ref GetKiRegenTimer(modPlayer);
                kiRegenTimer = 0;
                modPlayer.kiChargeRate += modPlayer.kiRegen;
            }
        }

        public override void PostUpdate()
        {
            if (MP_Unlock && Main.netMode == NetmodeID.MultiplayerClient)
            {
                MyPlayer modPlayer = Player.GetModPlayer<MyPlayer>();

                float mastery = modPlayer.masteryLevelLeg3;

                if (mastery >= 1f)
                {
                    LSSJ4Achieved = true;
                    MP_Unlock = false;
                }
            }
            if(LSSJ4Achieved && !LSSJ4UnlockMsg)
            {
                LSSJ4UnlockMsg = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(Language.GetText("Mods.DBTBalanceRevived.Misc.LSSJ4Unlocked"), Color.Green);
                    TransformationHandler.ClearTransformations(Player);
                    TransformationHandler.Transform(Player, TransformationHandler.GetTransformation("LSSJ4Buff").Value);
                }
            }

            if (BalanceConfigServer.Instance.KiRework)
            {
                if (Player.TryGetModPlayer(out MyPlayer modPlayer))
                {
                    modPlayer.kiChargeRate += modPlayer.kiRegen;
                }
            }
        }
        public override void ResetEffects()
        {
            if (upgradePathBuffer != null)
            {
                for (int i = 0; i < upgradePathBuffer.Length; i++)
                {
                    if (upgradePathBuffer[i] == ItemID.None)
                        continue;
                    Main.NewText(ItemLoader.GetItem(upgradePathBuffer[i]).Name);
                }
                for (int i = 0; i < upgradePathBuffer.Length; i++)
                    upgradePathBuffer[i] = 0;
            }
            if (BalanceConfigServer.Instance.KiRework)
            {
                MyPlayer modPlayer = Player.GetModPlayer<MyPlayer>();
                modPlayer.kiChargeRate += modPlayer.kiRegen;
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
            MyPlayer self = Player.GetModPlayer<MyPlayer>();
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
