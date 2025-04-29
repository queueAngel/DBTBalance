using DBTBalanceRevived.Helpers;
using DBZMODPORT.Buffs;
using Terraria.ModLoader;

namespace DBTBalanceRevived.Common
{
    public class DBTBalanceRevivedGlobalBuff : GlobalBuff
    {
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (BuffLoader.GetBuff(type) is TransBuff self)
            {
                tip = BuffHooks.BuildTooltip_Hook(self);
            }
        }
    }
}
