using DBTBalanceRevived.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace DBTBalanceRevived.Common
{
    public sealed class DBTBalanceRevivedGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!AccessoryHooks.UpgradePaths.TryGetValue((ushort)item.type, out var upgrades))
                return;
            if (player.TryGetModPlayer(out BPlayer modPlayer))
            {
                modPlayer.AddToUpgradesBuffer(item.type);
                for (int i = 0; i < upgrades.Length; i++)
                {
                    modPlayer.AddToUpgradesBuffer(upgrades[i]);
                }
            }
        }
    }
}
