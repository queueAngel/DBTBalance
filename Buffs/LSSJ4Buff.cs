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
            kiDrainRate = 4.5f;
            kiDrainRateWithMastery = 1.5f;
            attackDrainMulti = 1.70f;

            if (BalanceConfigServer.Instance.SSJTweaks)
            {
                damageMulti = 1.85f;
                speedMulti = 0.9f;
                baseDefenceBonus = 61;
            }
            else
            {
                damageMulti = 4.8f;
                speedMulti = 3.50f;
                baseDefenceBonus = 41;
            }
            base.SetStaticDefaults();
        }

        public override bool CanTransform(Player player)
        {
            var modPlayer = player.GetModPlayer<BPlayer>();
            return !player.HasBuff<LSSJ4Buff>() && modPlayer.LSSJ4Achieved && player.GetModPlayer<GPlayer>().Trait == "Legendary";
        }

        public override void OnTransform(Player player) => 
            player.GetModPlayer<BPlayer>().LSSJ4Active = true;
        public override void PostTransform(Player player) =>
            player.GetModPlayer<BPlayer>().LSSJ4Active = false;

        public override string FormName() => "Legendary Super Saiyan 4";

        public override bool Stackable() => false;

        public override Color TextColor() => Color.HotPink;

        public override SoundData SoundData() => new SoundData("DBZMODPORT/Sounds/SSJAscension", "DBZMODPORT/Sounds/SSJ3", 260);
        public override AuraData AuraData() => new AuraData("DBTBalance/Assets/LSSJ4Aura", 4, BlendState.AlphaBlend, new Color(250, 74, 67));
        public override Gradient KiBarGradient() => new Gradient(new Color(255, 56, 99)).AddStop(1f, new Color(156, 0, 34));

        public override string HairTexturePath() => "DBTBalance/Assets/LSSJ4Hair";

        public override bool SaiyanSparks() => true;

    }

    public class LSSJ4Panel : TransformationTree
    {
        public override bool Complete() => false;

        public override Connection[] Connections() => new Connection[]
        {
            new Connection(3,3,1,false,new Gradient(Color.LightGreen).AddStop(0.60f, new Color(255, 56, 99)))
        };

        public override string Name() => "LSSJ Partial Tree";

        public override Node[] Nodes() => new Node[]
        {
            new Node(4,3,"LSSJ4Buff","DBTBalance/Buffs/LSSJ4Buff","Only defeating a foe of cosmic proportions can unlock this power.",UnlockCondition,DiscoverCondition)
        };

        public bool UnlockCondition(Player player)
        {
            return player.GetModPlayer<BPlayer>().LSSJ4Achieved && player.GetModPlayer<GPlayer>().Trait == "Legendary";
        }
        public bool DiscoverCondition(Player player)
        {
            return player.GetModPlayer<GPlayer>().Trait == "Legendary";
        }
        public override bool Condition(Player player) => true;
    }
}
