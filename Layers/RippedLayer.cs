using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DBTBalance.Model
{
    public class RippedLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.ArmOverItem);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var drawPlayer = drawInfo.drawPlayer;

            if (drawPlayer.GetModPlayer<BPlayer>().LSSJ4Active && BalanceConfig.Instance.UseHair)
            {
                Color alphaArms = drawInfo.drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)(drawInfo.Position.X + drawInfo.drawPlayer.width * 0.5) / 16, (int)((drawInfo.Position.Y + drawInfo.drawPlayer.height * 0.25) / 16.0), Color.White), drawInfo.shadow);

                var xArms = (int)(drawInfo.Position.X - Main.screenPosition.X - (drawPlayer.bodyFrame.Width / 2) + (drawPlayer.width / 2));
                var yArms = (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f);

                Vector2 vel = drawPlayer.velocity - drawPlayer.oldVelocity;
                float offset = drawPlayer.wings != 0 ? (vel.Y > 0 ? -4.3f:0) : (drawPlayer.velocity.Y != 0 ? -4.3f : 0);
                DrawData dataArms = new DrawData(ModContent.Request<Texture2D>("DBTBalance/Assets/LSSJ4Arms", AssetRequestMode.AsyncLoad).Value,
                    new Vector2(xArms,yArms + offset) + drawPlayer.bodyPosition, 
                    drawPlayer.bodyFrame, 
                    alphaArms, 
                    drawPlayer.bodyRotation,
                    drawPlayer.bodyPosition,
                    1f, 
                    drawInfo.playerEffect, 0);

                drawInfo.DrawDataCache.Add(dataArms);
            }
        }
    }
}
