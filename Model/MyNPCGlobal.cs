using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using DBTBalance.Helpers;

namespace DBTBalance
{
    public class MyNPCGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void OnKill(NPC npc)
        {
            if (ModLoader.HasMod("DBZMODPORT"))
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    BPlayer Player = Main.CurrentPlayer.GetModPlayer<BPlayer>();
                    var ModPlayerClass = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
                    var getModPlayer = ModPlayerClass.GetMethod("ModPlayer");

                    var dbzPlayer = getModPlayer.Invoke(null, new object[] { Player.Player });
                    var masteryfield = ModPlayerClass.GetField("masteryLevelLeg3");
                    float mastery = (float)masteryfield.GetValue(dbzPlayer);

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
        }
    }
}
