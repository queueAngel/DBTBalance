using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using DBTBalance.Helpers;

namespace DBTBalance.Model
{
    public class MyGlobalBuff : GlobalBuff
    {
        public override void ModifyBuffTip(int type, ref string tip, ref int rare)
        {
            if(ModLoader.HasMod("DBZMODPORT"))
            {
                if(Detours.cachedTypes.TryGetValue(type,out string buffName))
                {
                    if (buffName != "UIBuff" && buffName != "UISignBuff" && buffName != "UEBuff")
                    {
                        var Transbuff = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("TransBuff"));
                        tip = (string)Transbuff.GetMethod("AssembleTransBuffDescription").Invoke(Detours.cachedBuffs[buffName], null);
                    }
                    else base.ModifyBuffTip(type, ref tip, ref rare);
                }
            }
        }
    }
}
