using LAP.Assets.TextureRegister;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.DrawNodes;
using UCA.Content.HitEffect;
using UCA.Content.MetaBalls;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class CosmicSlash : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public bool SpawnSlash;
        public int Time;
        public int MaxTime = 60;
        public int LaserLength;
        public Vector2 BeginPos;
        public Vector2 EndPos;
        public override void SetStaticDefaults()
        {
            Projectile.AddProtectedProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxTime;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.Opacity < 0.1f) return false;
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BeginPos, EndPos, 24f, ref _);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if(Projectile.LAP().FirstFrame)
            {
                FirstFrame();
            }
            Time++;
            if (Time > 6 && !SpawnSlash)
            {
                for (int i = 0; i < 260; i++)
                {
                    float Progress = i / 260f;
                    Vector2 SpawnPos = Vector2.Lerp(BeginPos, Projectile.Center, Progress) + Main.rand.NextVector2Circular(6, 6);
                    Vector2 spawnVec = Projectile.velocity * Main.rand.NextFloat(0.1f, 1.1f) * 18f + new Vector2(0, Main.rand.NextFloat(-0.2f, 0.2f));
                    CosmicMetaBall.SpawnLozengeParticle(SpawnPos, spawnVec, 1, 45);
                }
                for (int i = 0; i < 130; i++)
                {
                    float Progress = i / 130f;
                    Vector2 SpawnPos = Vector2.Lerp(EndPos, Projectile.Center, Progress) + Main.rand.NextVector2Circular(6, 6);
                    Vector2 spawnVec =- Projectile.velocity * Main.rand.NextFloat(0.1f, 1.1f) * 18f + new Vector2(0, Main.rand.NextFloat(-0.2f, 0.2f));
                    CosmicMetaBall.SpawnLozengeParticle(SpawnPos, spawnVec, 1, 45);
                }
                SpawnSlash = true;
                Projectile.timeLeft = 5;
            }
        }
        public void FirstFrame()
        {
            SoundEngine.PlaySound(SoundsMenu.SwordSwing2 with { Volume = 0.6f}, Projectile.Center);
            Projectile.rotation = Projectile.velocity.ToRotation();
            int Filp = Main.rand.NextBool() ? 1 : -1;
            new CosmicDustEmitting(Projectile.Center, Filp).Spawn();
            BeginPos = Projectile.Center + Projectile.velocity * 8 * 260;
            EndPos = Projectile.Center + -Projectile.velocity * 8 * 130;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                HitEffectManager.SpawnHitEffect(HitEffectManager.HEType<CosmicSlashHit>(), Projectile.owner, Projectile.GetSource_FromThis(), target.Center, Vector2.Zero);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 2, 5, Projectile.rotation, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
