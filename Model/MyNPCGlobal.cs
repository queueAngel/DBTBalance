using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using DBTBalanceRevived.Helpers;
using DBZMODPORT;

namespace DBTBalanceRevived
{
    public class MyNPCGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void OnKill(NPC npc)
        {
            if (npc.type != NPCID.MoonLordCore)
                return;

            foreach (var player in Main.ActivePlayers)
            {
                BPlayer bPlayer = player.GetModPlayer<BPlayer>();
                MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();

                if (!bPlayer.LSSJ4Achieved)
                {
                    if (Main.dedServ)
                    {
                        bPlayer.MP_Unlock = true;
                        bPlayer.LSSJ4Achieved = true;
                        BNetworkHandler.SendUnlockStatus(player.whoAmI, true);
                    }
                    else
                    {
                        if (myPlayer.masteryLevelLeg3 >= 1f)
                            bPlayer.LSSJ4Achieved = true;
                    }
                }
            }
        }
    }
}
