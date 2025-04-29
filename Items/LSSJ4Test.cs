using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBTBalance.Items
{
    public class LSSJ4Test : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /*
            Tooltip.SetDefault("DEUBG ITEM\bUnlocks Legenadry Super Saiyan 4.");
            DisplayName.SetDefault("LSSJ4 Test");
            */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Item.type] = 1;
        }
        public override void SetDefaults()
        {
            Item.color = new Color(0xC0, 0xC0, 0xCF);
            Item.width = 32;
            Item.height = 32;
            Item.value = 40000;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.defense = 0;
            Item.consumable = true;
        }
        public override bool ConsumeItem(Player player)
        {
            return true;
        }
        public override void OnConsumeItem(Player player)
        {
            player.GetModPlayer<BPlayer>().LSSJ4Achieved = true;
        }
        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<BPlayer>().LSSJ4Achieved = true;
            return true;
        }
    }
}
