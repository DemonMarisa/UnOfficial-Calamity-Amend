using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.PunishmentProj
{
    public class HolyJudgement : BaseRogueProj 
    {
        private ref float Counter => ref Projectile.ai[0];
        private float OriginalSpeed => Projectile.ai[1];
        private ref float Rotation => ref Projectile.ai[2];
        private float MountedX => Projectile.localAI[0];
        private float MountedY => Projectile.localAI[1];
        private float InitVec = 0;
        public override void ExSD()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            //这玩意是转起来的，所以实际dps会更少的，给他多点判定吧！！
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 80;

        }
        float opc = 1;
        public override void AI()
        {
            //刚加入时的初始化。
            if (Counter == 0)
            {
                SoundEngine.PlaySound(SoundsMenu.Misc_AngelBlast, Projectile.Center);
                InitVec = Projectile.velocity.ToRotation();
                Projectile.rotation = InitVec;
            }
            Lighting.AddLight(Projectile.Center, TorchID.White);
            Counter++;
            //让这个东西绕着转一会……
            Rotation += MathHelper.ToRadians(1.5f);
            //增加这个……转角。
            float curRot = InitVec + Rotation;
            //最后算速度。和一些别的。
            Projectile.velocity = curRot.ToRotationVector2() * OriginalSpeed;
            //转角处理。
            Projectile.rotation = Projectile.velocity.ToRotation();
            //维持悬挂让他跟随敌对单位
            Projectile.Center = new Vector2(MountedX, MountedY);
            if (Counter > 60)
            {
                opc -= 1f / 20f;
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            UCAUtilities.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 900, targetHitbox, 24);
        SpriteBatch SB { get => Main.spriteBatch; }
        public override bool PreDraw(ref Color lightColor)
        {
            //贴图。
            Texture2D warn = UCATextureRegister.Trail_ManaStreak.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int laserLengthScale = 35;
            //基础大小设定
            Vector2 baseScale = Projectile.scale * 0.32f * new Vector2(1, opc);
            DrawLaser(warn, drawPos, Color.Gold * opc, baseScale * new Vector2(laserLengthScale, 1.2f));
            DrawLaser(warn, drawPos, Color.Yellow * opc, baseScale * new Vector2(laserLengthScale, 0.8f));
            DrawLaser(warn, drawPos, Color.White * opc, baseScale * new Vector2(laserLengthScale, 0.4f));
            SB.End();
            SB.BeginDefault();
            return false;
        }
        private void DrawLaser(Texture2D warn, Vector2 drawPos, Color drawColor, Vector2 scale)
        {
            Vector2 ori = warn.Size() / 2 * new Vector2(0, 1);
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            UCAShaderRegister.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.Trail_ManaStreak.Size());
            UCAShaderRegister.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(120, UCATextureRegister.Trail_ManaStreak.Height()));
            UCAShaderRegister.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            UCAShaderRegister.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() *opc);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.4f);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0f);
            UCAShaderRegister.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            SB.Draw(warn, drawPos, null, drawColor, Projectile.rotation, ori, scale, SpriteEffects.None, 0);
        }
    }
}
