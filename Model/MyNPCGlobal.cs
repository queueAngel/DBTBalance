using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using DBTBalance.Helpers;
using DBZMODPORT;

namespace DBTBalance
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

            // Replaced with better code above - qAngel

            /*
            if (ModLoader.HasMod("DBZMODPORT"))
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    BPlayer Player = Main.CurrentPlayer.GetModPlayer<BPlayer>();
                    MyPlayer modPlayer = Main.CurrentPlayer.GetModPlayer<MyPlayer>();

                    float mastery = modPlayer.masteryLevelLeg3;

                    if (!Player.LSSJ4Achieved && mastery >= 1f)
                    {
                        if (npc.type == NPCID.MoonLordCore)
                        {
                            Player.LSSJ4Achieved = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[i];
                        if (player != null)
                        {
                            if (player.active)
                            {
                                BPlayer modPlayer = player.GetModPlayer<BPlayer>();

                                if (!modPlayer.LSSJ4Achieved)
                                {
                                    if (npc.type == NPCID.MoonLordCore)
                                    {
                                        modPlayer.MP_Unlock = true;
                                        modPlayer.LSSJ4Achieved = true;
                                        BNetworkHandler.SendUnlockStatus(i, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            */
        }
    }
}
