using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTBalance.Helpers;
using DBTBalance.Model;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using DBZGoatLib.Model;
using DBZGoatLib.Handlers;
using DBZGoatLib;
using Microsoft.Xna.Framework.Graphics;

namespace DBTBalance.Buffs
{
    public class LSSJ4Buff : Transformation
    {
        public override void SetStaticDefaults()
        {
            base.DisplayName.SetDefault("Legendary Super Saiyan 4");
            Main.buffNoTimeDisplay[Type] = true; 
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;

            damageMulti = 1f;
            speedMulti = 1.5f;
            kiDrainRate = 4.2f;
            kiDrainRateWithMastery = kiDrainRate / 2;
            attackDrainMulti = 4f;
            baseDefenceBonus = 22;
        }

        public static TransformationInfo LSSJ4Info => 
            new TransformationInfo(ModContent.BuffType<LSSJ4Buff>(), 
                "LSSJ4Buff", "Legendary Super Saiyan 4", 
                Color.HotPink, CanTransform , OnTransform, PostTransform, 
                new AnimationData(new AuraData("DBTBalance/Assets/LSSJ4Aura", 4, 3, BlendState.AlphaBlend,new Color(250, 74, 67), "DBTBalance/Assets/LSSJ4Hair"), true, 
                    new SoundData("DBZMODPORT/Sounds/SSJAscension", "DBZMODPORT/Sounds/SSJ3", 260)));

        
        public static bool CanTransform(Player player)
        {
            var modPlayer = player.GetModPlayer<BPlayer>();
            return !player.HasBuff<LSSJ4Buff>() &&
                player.HasBuff(DBTBalance.DBZMOD.Find<ModBuff>("LSSJ3Buff").Type) &&
                modPlayer.LSSJ4Achieved;
        }
        public static void OnTransform(Player player)
        {
            var modPlayer = player.GetModPlayer<BPlayer>();
            modPlayer.LSSJ4Active = true;

            var transMenu = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransMenu"));
            var menuSelection = transMenu.GetField("menuSelection");
            var none = Enum.Parse(menuSelection.FieldType, "None");
            menuSelection.SetValue(null, none);

        }
        public static void PostTransform(Player player)
        {
            var modPlayer = player.GetModPlayer<BPlayer>();
            modPlayer.LSSJ4Active = false;

            var transMenu = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransMenu"));
            var menuSelection = transMenu.GetField("menuSelection");
            dynamic none = Enum.Parse(menuSelection.FieldType, "LSSJ3");
            menuSelection.SetValue(null, none);

            modPlayer.Offset = null;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            var transMenu = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransMenu"));
            var menuSelection = transMenu.GetField("menuSelection");
            dynamic none = Enum.Parse(menuSelection.FieldType, "None");
            menuSelection.SetValue(null, none);
            base.Update(player, ref buffIndex);
        }
    }
}
