using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace DBTBalanceRevived.Model
{
    internal class BalanceConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static BalanceConfig Instance;

        [Header("LSSJ4Settings")]
        [DefaultValue(false)]
        public bool UseHair;
    }
    internal class BalanceConfigServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static BalanceConfigServer Instance;

        [Header("ToggleableBalanceAdjustments")]

        // this goes completely unused in the actual mod lol?
        [DefaultValue(true)]
        public bool ArmorCrunch;

        [DefaultValue(true)]
        public bool ChargeRework;

        [DefaultValue(true)]
        public bool KiRework;

        [DefaultValue(true)]
        public bool LongerTransform;

        [ReloadRequired]
        [DefaultValue(true)]
        public bool SSJTweaks;
    }
}
