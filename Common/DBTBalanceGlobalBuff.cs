using Terraria.ModLoader;

namespace DBTBalance.Common
{
    public class DBTBalanceGlobalBuff : GlobalBuff
    {
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            base.ModifyBuffText(type, ref buffName, ref tip, ref rare);
        }
    }
}
