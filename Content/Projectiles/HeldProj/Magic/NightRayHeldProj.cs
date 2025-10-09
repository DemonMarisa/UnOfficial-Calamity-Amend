using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.ItemOverride.Magic;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class NightRayHeldProj : BaseHeldProj
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<NightsRay>();
        public Vector2 RotVector => new Vector2(12 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;
        public override Vector2 Posffset => new Vector2(RotVector.X, RotVector.Y) * Owner.direction;
        public override float RotAmount => 0.25f;
        public override float RotOffset => MathHelper.PiOver4;

        public float ShaderOpacity = 0;

        public int AniProgress;

        public int InToAni = 10;
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool StillInUse()
        {
            return AniProgress >= InToAni && Main.mouseLeft;
        }

        public override void HoldoutAI()
        {
            if (AniProgress < InToAni)
                return;

            if (Main.mouseRight)
                return;

            if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                // 常规开火
                // 这里用发射的弹幕AI[0]是否为1来确定是否为主射线
                // ai[0]为1时是主射线
                Vector2 firePos = Projectile.Center + new Vector2(30, 0).RotatedBy(Projectile.rotation);

                GenUnDeathSign(firePos, 0.8f);

                SoundEngine.PlaySound(SoundsMenu.NightRayAttack, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), firePos, Projectile.rotation.ToRotationVector2() * 3, ModContent.ProjectileType<NightEnergy>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 1);
                Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.06f;

                // 在NPC周围发射一个十字的激光
                NightsRayOverride.UseCount++;
                if (NightsRayOverride.UseCount > 4)
                {
                    CrossFire();
                    NightsRayOverride.UseCount = 0;
                }
                UseDelay = Owner.HeldItem.useTime;

                for (int i = 0; i < 10; i++)
                {
                    Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                    new Line(firePos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 7), color, Main.rand.Next(60, 90), 0, 1, 0.1f, false, firePos).Spawn();
                }
            }
            else
            {
                Vector2 firePos = Projectile.Center + new Vector2(30, 0).RotatedBy(Projectile.rotation);
                if (UseDelay > Owner.HeldItem.useTime / 2)
                {
                    Vector2 SpawnPos = firePos + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(25, 75);
                    Vector2 SpawnPosToMouseWorld = (firePos - SpawnPos).SafeNormalize(Vector2.UnitX);
                    float rot = SpawnPosToMouseWorld.ToRotation() + 3;
                    Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                    new Line(SpawnPos, Vector2.Zero, color, Main.rand.Next(60, 90), rot, 1, 0.1f, true, firePos).Spawn();
                }
            }
        }

        public override bool CanDel()
        {
            return AniProgress == 0 && !Main.mouseLeft;
        }

        public override void PostAI()
        {
            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);

            if (!Main.mouseLeft)
            {
                AniProgress--;
                ShaderOpacity = MathHelper.Lerp(ShaderOpacity, 1f, 0.12f);
                return;
            }

            if (AniProgress < InToAni)
                AniProgress++;

            if (AniProgress < InToAni)
            {
                ShaderOpacity = MathHelper.Lerp(1f, 0, EasingHelper.EaseInCubic(AniProgress / (float)InToAni));
            }

            if (AniProgress >= InToAni)
            {
                ShaderOpacity = 0;
            }
        }

        public void CrossFire()
        {
            NPC npc = Projectile.FindClosestTarget(1500, true);
            float RandomOffset = Main.rand.NextFloat(0, MathHelper.TwoPi);

            if (npc is not null)
            {
                for (int i = 0; i < 4; i++)
                {
                    float DistanceToNPC = Vector2.Distance(Projectile.Center, npc.Center);
                    float PredictMult = DistanceToNPC / 48;
                    Vector2 CrossfirePos = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.PiOver2 * i).RotatedBy(RandomOffset) * 250;
                    Vector2 toNPCVector = (npc.Center + npc.velocity * PredictMult - CrossfirePos).SafeNormalize(Vector2.UnitX) * 1.5f;
                    GenUnDeathSign(CrossfirePos, 0.4f);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), CrossfirePos, toNPCVector, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    Main.projectile[p].tileCollide = false;
                    Main.projectile[p].penetrate = 1;
                }
            }
            else
            {
                Vector2 RandomPos = new(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));
                for (int i = 0; i < 4; i++)
                {
                    Vector2 CrossRandomfirePos = RandomPos + Vector2.UnitX.RotatedBy(MathHelper.PiOver2 * i).RotatedBy(RandomOffset) * 250;
                    Vector2 toPosVector = (RandomPos - CrossRandomfirePos).SafeNormalize(Vector2.UnitX) * 1.5f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + CrossRandomfirePos, toPosVector, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    Main.projectile[p].tileCollide = false;
                    Main.projectile[p].penetrate = 1;
                }
            }
        }

        public static void GenUnDeathSign(Vector2 firePos, float speedMult = 1)
        {
            // 生成星形
            for (int i = 0; i < 60; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i / 60f;

                // Parametric equations for an asteroid.
                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 5D);
                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 5D);

                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 7f * speedMult;

                ShadowMetaBall.SpawnParticle(firePos,
                    puffDustVelocity,
                    0.13f);
            }

            // 生成四条线
            for (int i = 0; i < 6; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i / 4f + MathHelper.PiOver4;
                Vector2 vector = offsetAngle.ToRotationVector2() * 4 * speedMult;
                for (int j = 0; j < 10; j++)
                {
                    ShadowMetaBall.SpawnParticle(firePos, vector + vector * (j / 10f), 0.15f);
                }
            }

            // 生成四条线的切线
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Vector2 beginVector = new(1, 0.3f);
                    Vector2 endVector = new(1, -0.3f);
                    Vector2 vector = Vector2.Lerp(beginVector, endVector, j / 5f);
                    ShadowMetaBall.SpawnParticle(firePos, vector.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver2 * i) * 5.7f * speedMult, 0.15f);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.controlLeft = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
        public override bool ExtraPreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.EdgeMeltsShader.Parameters["progress"].SetValue(ShaderOpacity);
            UCAShaderRegister.EdgeMeltsShader.Parameters["InPutTextureSize"].SetValue(ModContent.Request<Texture2D>(Texture).Size());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeColor"].SetValue(Color.DarkViolet.ToVector4());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeWidth"].SetValue(0.01f);
            UCAShaderRegister.EdgeMeltsShader.CurrentTechnique.Passes[0].Apply();

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f) + RotOffset * Owner.direction;

            Vector2 rotationPoint = RotPoint;

            SpriteEffects flipSprite = Owner.direction * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
