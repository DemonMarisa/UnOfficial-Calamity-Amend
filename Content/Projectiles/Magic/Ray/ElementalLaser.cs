using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.BaseClass;
using LAP.Core.Graphics;
using UCA.Core.Utilities;
using LAP.Core.Utilities;
using LAP.Core.Enums;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ElementalLaser : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public float Opacity = 0f;
        public int MaxLife = 75;
        public float LaserLength = 0;
        public Vector2 BeginPos;
        public Vector2 EndPos;
        public int LaserTimeOffset;
        public List<Vector2> FirePos = [];
        public ref float WeaponStates => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxLife;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Opacity < 0.1f)
                return false;
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float _ = float.NaN;
            Vector2 beamEndPos = BeginPos + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserLength;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BeginPos, beamEndPos, 24f, ref _);
            return c;
        }

        public override void AI()
        {
            UpdateMisc();
            FirstFrame();
            UpdateLaserLength();
            if (Projectile.timeLeft == 40)
            {
                ShootStarDust();
            }
        }
        #region 初始化
        public void FirstFrame()
        {
            if (!Projectile.LAP().FirstFrame)
                return;
            LaserTimeOffset = Main.rand.Next(0, 100);
            BeginPos = Projectile.Center;
            EndPos = Projectile.Center;
            if (WeaponStates == ElementalRayState.StarDust)
            {
                FirePos.Add(Projectile.Center);
                NPC npc = Projectile.FindClosestTarget(1500, false);
                if (npc != null)
                {
                    Vector2 ToNPCVel = (npc.Center - EndPos).SafeNormalize(Projectile.rotation.ToRotationVector2());
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), EndPos, ToNPCVel * 12, ModContent.ProjectileType<StarDustLaser>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 1);
                }
            }
            #region 生成辉光
            Vector2 Posoffset = new Vector2(10, 0).RotatedBy(Projectile.rotation);
            if (WeaponStates == ElementalRayState.Solar)
            {
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.OrangeRed, 60, 1f, 0.4f).Spawn();
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.Orange, 60, 1f, 0.4f).Spawn();
            }
            else if (WeaponStates == ElementalRayState.Nebula)
            {
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.Violet, 60, 1f, 0.4f).Spawn();
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.BlueViolet, 60, 1f, 0.4f).Spawn();
            }
            else if (WeaponStates == ElementalRayState.Vortex)
            {
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.Turquoise, 60, 1f, 0.4f).Spawn();
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.DarkTurquoise, 60, 1f, 0.4f).Spawn();
            }
            else if (WeaponStates == ElementalRayState.StarDust)
            {
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.SkyBlue, 60, 1f, 0.4f).Spawn();
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.DeepSkyBlue, 60, 1f, 0.4f).Spawn();
            }
            else
                new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.White, 60, 1f, 0.4f).Spawn();
            for (int i = 0; i < 35; i++)
            {
                Color RandomColor;

                if (WeaponStates == ElementalRayState.Solar)
                    RandomColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat(0, 1));
                else if (WeaponStates == ElementalRayState.Nebula)
                    RandomColor = Color.Lerp(Color.BlueViolet, Color.Violet, Main.rand.NextFloat(0, 1));
                else if (WeaponStates == ElementalRayState.Vortex)
                    RandomColor = Color.Lerp(Color.DarkTurquoise, Color.Turquoise, Main.rand.NextFloat(0, 1));
                else if (WeaponStates == ElementalRayState.StarDust)
                    RandomColor = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, Main.rand.NextFloat(0, 1));
                else
                    RandomColor = Color.Lerp(Color.White, Color.AntiqueWhite, Main.rand.NextFloat(0, 1));

                new MediumGlowBall(Projectile.Center + Posoffset, RandomColor, 120, 0.2f, Main.rand.NextFloat(2f, 3f)).Spawn();
            }
            #endregion
            #region 生成伴随主弹幕的树
            Vector2 firVec = Projectile.velocity.SafeNormalize(Vector2.Zero) * 3f;
            Vector2 ProjFireOffset = new Vector2(-24, 0).RotatedBy(Projectile.velocity.ToRotation());
            Vector2 firPos = Projectile.Center + ProjFireOffset;
            int Filp = Main.rand.NextBool() ? 1 : -1;
            for (int i = 0; i < 2; i++)
            {
                if (WeaponStates == ElementalRayState.Solar)
                {
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.OrangeRed, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(2, 5), -1 * Filp, Main.rand.NextFloat(9, 18f)).Spawn();
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.Orange, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(3, 6), 1 * Filp, Main.rand.NextFloat(11, 22)).Spawn();
                }
                else if (WeaponStates == ElementalRayState.Nebula)
                {
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.Violet, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(2, 5), -1 * Filp, Main.rand.NextFloat(9, 18f)).Spawn();
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.BlueViolet, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(3, 6), 1 * Filp, Main.rand.NextFloat(11, 22)).Spawn();
                }
                else if (WeaponStates == ElementalRayState.Vortex)
                {
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.Turquoise, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(2, 5), -1 * Filp, Main.rand.NextFloat(9, 18f)).Spawn();
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.DarkTurquoise, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(3, 6), 1 * Filp, Main.rand.NextFloat(11, 22)).Spawn();
                }
                else if (WeaponStates == ElementalRayState.StarDust)
                {
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.SkyBlue, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(2, 5), -1 * Filp, Main.rand.NextFloat(9, 18f)).Spawn();
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.DeepSkyBlue, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(3, 6), 1 * Filp, Main.rand.NextFloat(11, 22)).Spawn();
                }
                else
                {
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.White, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(2, 5), -1 * Filp, Main.rand.NextFloat(9, 18f)).Spawn();
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.GhostWhite, 0, DrawLayer.AfterDusts, Main.rand.NextFloat(3, 6), 1 * Filp, Main.rand.NextFloat(11, 22)).Spawn();
                }
            }
            #endregion
        }
        #endregion
        #region 更新激光长度
        public void UpdateLaserLength()
        {
            if (Projectile.timeLeft > MaxLife - 15)
            {
                LaserLength = (EndPos - BeginPos).Length();
                EndPos += new Vector2(128, 0).RotatedBy(Projectile.rotation);
                if (WeaponStates == ElementalRayState.Solar || WeaponStates == ElementalRayState.Nebula)
                    return;
                if (Projectile.timeLeft % 2 == 0)
                {
                    if (WeaponStates == ElementalRayState.StarDust)
                    {
                        NPC npc = Projectile.FindClosestTarget(1500, false);
                        if (npc != null)
                        {
                            Vector2 ToNPCVel = (npc.Center - EndPos).SafeNormalize(Projectile.rotation.ToRotationVector2());
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), EndPos, ToNPCVel * 12, ModContent.ProjectileType<StarDustLaser>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                        }
                        else
                        {
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), EndPos, Vector2.Zero, ModContent.ProjectileType<StarDustLaser>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    FirePos.Add(EndPos);
                }
            }
        }
        #endregion
        #region 更新杂项
        public void UpdateMisc()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft > MaxLife / 2)
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.2f);
            else
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.15f);
        }
        #endregion
        #region 发射星辰碎块
        public void ShootStarDust()
        {
            if (FirePos.Count == 0)
                return;

            for (int i = 0; i < FirePos.Count; i++)
            {
                NPC npc = Projectile.FindClosestTarget(1500, false);
                for (int j = 0; j < 20; j++)
                {
                    Color RandomColor;
                    RandomColor = Color.Lerp(Color.DarkBlue, Color.SkyBlue, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(FirePos[i], RandomColor, 60, 0.2f, Main.rand.NextFloat(1.6f, 2f)).Spawn();
                }
                int type = ModContent.ProjectileType<StarDustFragment>();
                if (WeaponStates == ElementalRayState.Vortex)
                    type = ModContent.ProjectileType<VortexLightning>();

                if (npc != null)
                {
                    float DistanceToNPC = Vector2.Distance(FirePos[i], npc.Center);
                    float PredictMult = DistanceToNPC / 45;
                    Vector2 ToNPCVel = (npc.Center - FirePos[i] + npc.velocity * PredictMult).SafeNormalize(Projectile.rotation.ToRotationVector2());
                    if (WeaponStates == ElementalRayState.Vortex)
                        ToNPCVel = (npc.Center - FirePos[i]).SafeNormalize(Projectile.rotation.ToRotationVector2());
                 
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), FirePos[i], ToNPCVel * 24, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                else
                {
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), FirePos[i], Vector2.Zero, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                Color SpawnColor = Color.SkyBlue;
                new CrossGlow(FirePos[i], Vector2.Zero, SpawnColor, 25, 1f, 0.3f).Spawn();
            }
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (WeaponStates == ElementalRayState.StarDust)
                return;
            if (Projectile.LAP().OnceHitEffect)
            {
                int NebulaEnergyCount = 6;
                if (WeaponStates == ElementalRayState.Solar || WeaponStates == ElementalRayState.Vortex)
                    NebulaEnergyCount = 0;
                if (WeaponStates == ElementalRayState.Nebula)
                    NebulaEnergyCount = 10;
                 
                int Type = ModContent.ProjectileType<NebulaEnergy>();
                if (WeaponStates == ElementalRayState.Solar)
                    Type = ModContent.ProjectileType<SolarFireBall>();
                if (WeaponStates == ElementalRayState.Vortex)
                    Type = ModContent.ProjectileType<VortexMissle>();

                for (int i = 0; i < NebulaEnergyCount; i++)
                {
                    Vector2 randomOffset = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * (75 + target.width / 2) * Main.rand.NextFloat(0.6f, 1.2f);
                    Vector2 ToNPCVel = LAPUtilities.GetVector2(target.Center, target.Center + randomOffset).SafeNormalize(Vector2.Zero) * 9 * Main.rand.NextFloat(0.9f, 1.2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + randomOffset, ToNPCVel.RotatedByRandom(MathHelper.PiOver4), Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                if (WeaponStates == ElementalRayState.Nebula || WeaponStates == ElementalRayState.Vortex)
                    return;
                if (WeaponStates == ElementalRayState.Solar)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<SolarBlast>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner, 10, 0.2f, 1);
                else
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<SolarBlast>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner);
            }
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.Additive);

            if (WeaponStates == ElementalRayState.Solar)
                DrawLaser(Color.OrangeRed, 0.25f, 0.1f, -100);
            else
                DrawLaser(Color.Gray, 0.25f, 0.1f, -100);

            LAPUtilities.ReSetToBeginShader();
            DrawLaser(Color.White, 0.15f, 0.1f, -50);
            DrawLaser(Color.White, 0.07f, 0.02f , -100);
            LAPUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawLaser(Color colro, float height = 0.2f, float op = 0.1f, int Speed = -50)
        {
            float TextureHeight = UCATextureRegister.ElementalRayFlow.Height();
            float TextureWidth = UCATextureRegister.ElementalRayFlow.Width();
            Effect shader = UCAShaderRegister.StandardFlowShader.Value;
            shader.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.ElementalRayFlow.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(LaserLength, TextureHeight));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed + LaserTimeOffset);
            shader.Parameters["uColor"].SetValue(colro.ToVector4() * Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(op);
            shader.Parameters["uFadeinLength"].SetValue(op);
            shader.CurrentTechnique.Passes[0].Apply();

            Vector2 orig = new(0, TextureHeight / 2);
            float xScale = LaserLength / TextureWidth;
            Main.spriteBatch.Draw(UCATextureRegister.TerrarRayFlow.Value, BeginPos - Main.screenPosition, null, Color.White, Projectile.rotation, orig, new Vector2(xScale, height), SpriteEffects.None, 0);
        }
    }
}
