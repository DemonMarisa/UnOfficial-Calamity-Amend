using LAP.Assets.Effects;
using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Content.Configs;
using LAP.Content.Particles;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Keybind;
using LAP.Core.LAPSource;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Melee.GreatSword;
using UCA.Content.Projectiles.Melee.NormalProj;
using static System.Net.Mime.MediaTypeNames;

namespace UCA.Content.Projectiles.HeldProj.Melee.StormRuler
{
    public class StormRulerHeldSwingProj : ModProjectile, ILocalizedModType, IPixelatedRenderer
    {
        public static SoundStyle WindAttack1 => new($"{LAPSoundsMenu.AttackSoundRoute}/WindAttack1") { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 };
        public static SoundStyle WindAttack2 => new($"{LAPSoundsMenu.AttackSoundRoute}/WindAttack2") { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 };
        public DrawLayer LayerToRenderTo => DrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<StormRulerAlt>();
        public override string Texture => UCATextureRegister.StormRulerAlt.Path;
        public Player Owner => Projectile.Owner();
        public ref float Filp => ref Projectile.ai[0];
        public ref float Heigh => ref Projectile.ai[1];
        public ref float BeginTargetRot => ref Projectile.ai[2];
        public int UseTime => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * 10, 200);
        public AniHelper AniHelper = new AniHelper(3);
        public List<Vector2> OldAimPos = [];
        public float SwordLength = 170;
        public float ScaleMult = 1.25f;
        public float TargetRot = 0;
        public float ProjRotOffset;
        public float SlashOpacity = 1f;
        public override void SetStaticDefaults()
        {
            Projectile.AddHeldProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 450;
            Projectile.noEnchantmentVisuals = true;
            Projectile.netImportant = true;
            Projectile.extraUpdates = 10;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            if (Projectile.LAP().FirstFrame)
                return false;
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * SwordLength * 1.1f * Projectile.scale;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, 24f, ref _);
            return c;
        }
        public override void AI()
        {
            Init();
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Projectile.SetHeldProj(Owner, true, false);
            Projectile.Center = Owner.Center + new Vector2(-2, -2);
            Projectile.timeLeft = 2;
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                AniHelper.UpDateAni(AniState.Begin);
                HandleBeginAni();
            }
            else if (!AniHelper.HasFinish[AniState.End])
            {
                TargetRot = TargetRot.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), 0.1f);
                AniHelper.UpDateAni(AniState.End);
                HandleEndAni();
            }
            else
            {
                Projectile.Kill();
            }
            Projectile.velocity = TargetRot.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            if (OldAimPos.Count > 100)
                OldAimPos.RemoveAt(0);
            Owner.SetArmRot(Projectile.rotation);
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.SwingAttack with { Volume = 0.85f, Pitch = Main.rand.NextFloat(0.2f, 0.4f) , MaxInstances = -1}, Projectile.Center);
                AniHelper.MaxAniProgress[AniState.Begin] = (int)(UseTime * 0.33f);
                AniHelper.MaxAniProgress[AniState.End] = (int)(UseTime * 0.66f);
                TargetRot = BeginTargetRot;
                SwordLength = 140;
                SlashOpacity = 1f;
            }
        }
        public void HandleBeginAni()
        {
            float easedProgress = EasingHelper.EaseInBack(AniHelper.GetProgress(AniState.Begin));
            float baseRotation = AniHelper.UpDateAngle(-135 * Filp, 105 * Filp, Owner.direction, easedProgress);
            Matrix transform = Matrix.CreateRotationZ(baseRotation) * Matrix.CreateScale(1.2f, Heigh, 1f);
            Vector2 TargetPos = Vector2.Transform(Vector2.UnitX, transform) * 1.25f;
            Projectile.scale = TargetPos.Length();
            Projectile.rotation = TargetPos.ToRotation() + TargetRot;
            if (AniHelper.AniProgress[AniState.Begin] == 60)
            {
                // SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.Center);
                if (Projectile.IsLocalPlayer())
                {
                    Vector2 fireVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 42;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - fireVel * 3.5f, fireVel, ProjectileType<StormShockWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Heigh);
                }
            }
            if (easedProgress < 0.01f)
            {
                TargetRot = TargetRot.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), 0.01f);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 Pos = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos.RotatedBy(TargetRot) * 160, Main.rand.NextFloat(0.5f, 1f));
                    new CampSmoke(Pos, Owner.velocity * Main.rand.NextFloat(0f, 1.5f), Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.4f, Main.rand.NextFloat(0.15f, 0.3f)).Spawn();
                }
                Vector2 Pos2 = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos.RotatedBy(TargetRot) * 160, Main.rand.NextFloat(0.35f, 1f));
                new SmallGlowBall(Pos2, Vector2.Zero, Color.White, Main.rand.Next(30, 120), 0.1f, 3f).Spawn();

                float baseSlashRotation = AniHelper.UpDateAngle(-135 * Filp, 145 * Filp, Owner.direction, easedProgress);
                Matrix Slashtransform = Matrix.CreateRotationZ(baseSlashRotation) * Matrix.CreateScale(1.2f, Heigh, 1f);
                Vector2 SlashTargetPos = Vector2.Transform(Vector2.UnitX, Slashtransform) * 1.25f;
                Vector2 FinalPos = SlashTargetPos.RotatedBy(TargetRot) * SwordLength;
                OldAimPos.Add(FinalPos);
                if (Main.rand.NextBool(2))
                {
                    Vector2 beginSpawnPos = Owner.Center;
                    Vector2 EndSpawnPos = Owner.Center + FinalPos;
                    Vector2 SpawnPos = Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat());
                    Vector2 firVel = Vector2.UnitX.RotatedBy(baseSlashRotation + TargetRot + MathHelper.PiOver2) * 6 * Filp * Owner.direction;
                    Color DrawColor = Color.White;
                    new TrailGlowBall(SpawnPos, firVel, DrawColor * 0.5f, Main.rand.Next(45, 65), 0.2f, true).Spawn();
                }
            }
        }
        public void HandleEndAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.End));
            float baseRotation = AniHelper.UpDateAngle(105 * Filp, 135 * Filp, Owner.direction, easedProgress);
            Matrix transform = Matrix.CreateRotationZ(baseRotation) * Matrix.CreateScale(1.2f, Heigh, 1f);
            Vector2 TargetPos = Vector2.Transform(Vector2.UnitX, transform) * 1.25f;
            Projectile.scale = TargetPos.Length();
            Projectile.rotation = TargetPos.ToRotation() + TargetRot;
            SlashOpacity = 1f - AniHelper.GetProgress(AniState.End);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, -2 * -Owner.direction, 5, MathHelper.TwoPi, 0.5f, true, 1000);
        }
        public override void OnKill(int timeLeft)
        {
            if (!Projectile.IsLocalPlayer())
                return;
            if (LAPKeybind.WeaponSkillHotKey.Current)
            {
                Item item = ItemLoader.GetItem(ItemType<StormRulerAlt>()).Item;
                EntitySource_ItemUse_WeaponSkill source = new (Owner, item);
                if (!Owner.HasProj<StormRulerHeldSkillProj>())
                    Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeldSkillProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            else if (Main.mouseLeft)
            {
                if (Filp == 1)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, -1, Main.rand.NextFloat(0.5f, 0.8f), TargetRot);
                else
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 1, Main.rand.NextFloat(0.5f, 0.8f), TargetRot);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.LAP().FirstFrame)
                return false;
            PixelatedRenderManger.BeginDrawProj = true;
            DrawBlade(lightColor);
            return false;
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            LAPContent.ReSetToBeginShader_Pixel(BlendState.Additive);
            Texture2D texture = LAPTextureRegister.StandardGradient.Value;
            Effect effect = LAPShaderRegister.AlphaFade.Value;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.White * 0.3f, 0.95f);
            DrawSlash(texture, Color.White * 0.6f, 0.7f);
            DrawSlash(texture, Color.White * 0.4f, 0.3f);
            DrawSlash(texture, Color.White * 0.3f, 0f);
            if (!LAPConfig.Instance.PerformanceMode)
            {
                Texture2D texture2 = UCATextureRegister.Aura_01.Value;
                Effect effect2 = LAPShaderRegister.AlphaFade_Noise_OColor.Value;
                effect2.Parameters["uFadeoutLeftLength"].SetValue(0.2f);
                effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
                effect2.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.3f, 0));
                effect2.Parameters["UVMult"].SetValue(new Vector2(2f, 2f));
                effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
                effect2.CurrentTechnique.Passes[0].Apply();
                DrawSlash(texture2, Color.White, 0.4f);
                texture2 = UCATextureRegister.Slash.Value;
                DrawSlash(texture2, Color.White, 0.4f);
            }
            LAPContent.ReSetToEndShader_Pixel();
        }
        public void DrawBlade(Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D _, out Vector2 _, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            Texture2D texture = UCATextureRegister.StormRulerAlt.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * -9;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * ScaleMult, flipSprite, 0f);
        }
        public void DrawSlash(Texture2D texture,Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Head, drawcolor * SlashOpacity, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Source, drawcolor * SlashOpacity, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
