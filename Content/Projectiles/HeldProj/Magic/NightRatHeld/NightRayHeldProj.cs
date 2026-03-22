using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld
{
    public class NightRayHeldProj : BaseHeldProj
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<NightsRayAlt>();
        public Vector2 RotVector => new Vector2(12 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 PositionOffset => RotVector * Owner.direction;
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.Opacity = 0f;
           RotAmount = 0.25f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.Opacity);
            writer.Write(Projectile.ai[2]);
            writer.Write(Projectile.ai[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.Opacity = reader.ReadSingle();
            Projectile.ai[2] = reader.ReadSingle();
            Projectile.ai[1] = reader.ReadSingle();
        }
        public override void ExAI()
        {
            RotAmount = 0.25f;
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.12f);
            if (!Owner.LAP().MouseLeft && Owner.LAP().MouseRight)
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.12f);
            else
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.12f);
            if (Owner.LAP().MouseLeft && !Owner.LAP().MouseRight)
            {
                if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                {
                    Vector2 firePos = Projectile.Center + new Vector2(30, 0).RotatedBy(Projectile.rotation);
                    UseDelay = Owner.HeldItem.useTime;
                    // 常规开火
                    // 这里用发射的弹幕AI[0]是否为1来确定是否为主射线
                    // ai[0]为1时是主射线
                    if (Projectile.IsLocalPlayer())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), firePos, Projectile.rotation.ToRotationVector2() * 3, ModContent.ProjectileType<NightEnergy>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.8f);
                        NightsRayAlt.UseCount++;
                        if (NightsRayAlt.UseCount > 4)
                        {
                            CrossFire();
                            NightsRayAlt.UseCount = 0;
                        }
                    }
                    Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.1f;
                    UseDelay = Owner.HeldItem.useTime;
                }
            }
            if (UseDelay > Owner.HeldItem.useTime / 2)
            {
                Vector2 firePos = Projectile.Center + new Vector2(30, 0).RotatedBy(Projectile.rotation);
                Vector2 SpawnPos = firePos + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(25, 75);
                Vector2 SpawnPosToMouseWorld = (firePos - SpawnPos).SafeNormalize(Vector2.UnitX);
                float rot = SpawnPosToMouseWorld.ToRotation() + 3;
                Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                new Line(SpawnPos, Vector2.Zero, color, Main.rand.Next(60, 90), rot, 1, 0.1f, true, firePos).Spawn();
            }
        }
        public override void PostAI()
        {
            base.PostAI();
            if (!Owner.LAP().MouseLeft && Owner.LAP().MouseRight && UseDelay == 0)
            {
                Projectile.Kill();
            }
            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
        }

        public void CrossFire()
        {
            NPC npc = LAPUtilities.FindClosestTarget(Projectile.Center,1500, false);
            float RandomOffset = Main.rand.NextFloat(0, MathHelper.TwoPi);
            if (npc is not null)
            {
                for (int i = 0; i < 4; i++)
                {
                    float DistanceToNPC = Vector2.Distance(Projectile.Center, npc.Center);
                    float PredictMult = DistanceToNPC / 48;
                    Vector2 CrossfirePos = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.PiOver2 * i).RotatedBy(RandomOffset) * 250;
                    Vector2 toNPCVector = (npc.Center + npc.velocity * PredictMult - CrossfirePos).SafeNormalize(Vector2.UnitX) * 1.5f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), CrossfirePos, toNPCVector, ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.4f);
                        Main.projectile[p].tileCollide = false;
                        Main.projectile[p].penetrate = 1;
                    }
                }
            }
            else
            {
                Vector2 RandomPos = new(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));
                for (int i = 0; i < 4; i++)
                {
                    Vector2 CrossRandomfirePos = RandomPos + Vector2.UnitX.RotatedBy(MathHelper.PiOver2 * i).RotatedBy(RandomOffset) * 250;
                    Vector2 toPosVector = (RandomPos - CrossRandomfirePos).SafeNormalize(Vector2.UnitX) * 1.5f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + CrossRandomfirePos, toPosVector, ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.4f);
                        Main.projectile[p].tileCollide = false;
                        Main.projectile[p].penetrate = 1;
                    }
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
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.DarkViolet, 0.01f, 0);

            Projectile.GetProjDrawInfo_Staff(out Texture2D _, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
