using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace DBTBalance.Model
{
    [Label("Client Settings")]
    internal class BalanceConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static BalanceConfig Instance;

        [Header("Legendary Saiyan 4 Settings")]
        [Label("Replace Hair/Arm with hair:")]
        [Tooltip("Replace the player's chest piece with red-ish hair.")]
        [DefaultValue(true)]
        public bool UseHair;


    }
}
