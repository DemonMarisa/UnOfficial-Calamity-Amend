using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.ThunderProj
{
    public class ThunderPlatform : RogueProjClass
    {
        public override string Texture => (GetType().Namespace + "." + GetType().Name).Replace(".", "/");
        public int SmashHammerIndex
        {  
           get => (int)Projectile.ai[0];
           set => Projectile.ai[0] = value; 
        }
        public int KilledSignal
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public bool ShouldKillDraw
        {
            get => Projectile.ai[2] == 1f;
            set => Projectile.ai[2] = value ? 1f : 0f; 
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.ignoreWater = true;
            //假平台撞到物块本身需要做判定
            Projectile.tileCollide = true;
            Projectile.timeLeft = 3000;
            Projectile.extraUpdates = 0;
        }
        //同理，这东西不会受到任何伤害
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            //此处时刻更新eu避免某些神秘模组的修改
            Projectile.extraUpdates = 0;
            Main.NewText(Projectile.Center);
            //假平台唯一需要做的事情是判定玩家距离等，过远或者之类的处死。
            if (Owner.dead || KilledSignal == 1)
                Projectile.Kill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //触地不会处死，但是会做掉假平台的绘制
            ShouldKillDraw = true;
            return false;
        }
        SpriteBatch SB { get => Main.spriteBatch; }
        public override bool PreDraw(ref Color lightColor)
        {
            if (ShouldKillDraw)
                return false;
            Texture2D tex = Projectile.GetTexture();
            Vector2 scale = new Vector2(1f, 1f);
            Vector2 orig = tex.Size() / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            Main.spriteBatch.Draw(tex, drawPos, null, Color.White, 0, orig, scale, 0, 0.1f);
            SB.End();
            SB.BeginDefault();
            return false;
        }
    }
}
