using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DBTBalance.Model
{
    public class BuffLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Torso);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            BPlayer player = drawInfo.drawPlayer.GetModPlayer<BPlayer>();

            if (player.LSSJ4Active && BalanceConfig.Instance.UseHair)
            {
                Color alphaBody = drawInfo.drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)(drawInfo.Position.X + drawInfo.drawPlayer.width * 0.5) / 16, (int)((drawInfo.Position.Y + drawInfo.drawPlayer.height * 0.25) / 16.0), Color.White), drawInfo.shadow);

                var xBody = (int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2));
                var yBody = (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f);
                DrawData dataBody = new DrawData(ModContent.Request<Texture2D>("DBTBalance/Assets/LSSJ4Body", AssetRequestMode.AsyncLoad).Value,
                    new Vector2(xBody, yBody) + drawInfo.drawPlayer.bodyPosition, 
                    drawInfo.drawPlayer.bodyFrame,
                    alphaBody, 
                    drawInfo.drawPlayer.bodyRotation, 
                    drawInfo.drawPlayer.bodyPosition, 
                    1f, 
                    drawInfo.playerEffect, 0);

                Color alphaArms = drawInfo.drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)(drawInfo.Position.X + drawInfo.drawPlayer.width * 0.5) / 16, (int)((drawInfo.Position.Y + drawInfo.drawPlayer.height * 0.25) / 16.0), Color.White), drawInfo.shadow);

                var xArms = (int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2));
                var yArms = (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f);
                DrawData dataArms = new DrawData(ModContent.Request<Texture2D>("DBTBalance/Assets/LSSJ4Arms", AssetRequestMode.AsyncLoad).Value,
                    new Vector2(xArms,yArms) + drawInfo.drawPlayer.bodyPosition, 
                    drawInfo.drawPlayer.bodyFrame, 
                    alphaArms, 
                    drawInfo.drawPlayer.bodyRotation,
                    drawInfo.drawPlayer.bodyPosition,
                    1f, 
                    drawInfo.playerEffect, 0);

                drawInfo.DrawDataCache.Add(dataArms);
                drawInfo.DrawDataCache.Add(dataBody);
            }
        }
    }
}
