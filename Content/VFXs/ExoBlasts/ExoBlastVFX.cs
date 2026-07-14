using LAP.Assets.TextureRegister;
using LAP.Content.Particles_ECS;
using LAP.Core.Enums;
using LAP.Core.Graphics.VFX;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Content.Particiles;

namespace UCA.Content.VFXs.ExoBlasts
{
    public class ExoBlastVFX : VFXBehavior
    {
        public static void Spawn(Vector2 Center, float rot = 0)
        {
            int Type = LAPContent.VFXType<ExoBlastVFX>();
            VFXInstance vfx = LAPContent.SpawnVFX(Type, Center, Vector2.Zero, Color.White);
            vfx.AiFloat[0] = rot;
        }
        public override DrawLayer Layer => DrawLayer.AfterDusts;
        public override BlendState BlendState => BlendState.Additive;
        public float SpawnPointRot => VFXInstance.AiFloat[0];

        public bool FirstFrame = true;
        public Vector2[] SpawnEnergyPoint = new Vector2[8];
        public override void OnSpawn()
        {
            VFXInstance.Scale = 1.1f;
            VFXInstance.Lifetime = 60;
        }
        public override void Update()
        {
            if (FirstFrame)
            {
                SpawnEnergyPoint = new Vector2[8];
                Vector2 targetCent = VFXInstance.Position;
                SpawnEnergyPoint[0] = targetCent + new Vector2(40, 120).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                SpawnEnergyPoint[1] = targetCent - new Vector2(40, 120).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                SpawnEnergyPoint[2] = targetCent - new Vector2(100, -110).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                SpawnEnergyPoint[3] = targetCent + new Vector2(100, -110).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                SpawnEnergyPoint[4] = targetCent - new Vector2(90, -90).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                SpawnEnergyPoint[5] = targetCent + new Vector2(90, -90).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                SpawnEnergyPoint[6] = targetCent - new Vector2(140, 0).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                SpawnEnergyPoint[7] = targetCent + new Vector2(140, 0).RotatedBy(SpawnPointRot) * VFXInstance.Scale;
                FirstFrame = false;
                for (int i = 0; i < SpawnEnergyPoint.Length; i++)
                {
                    Vector2 pos = SpawnEnergyPoint[i];
                    Color color = Color.LimeGreen;
                    if (i == 2 || i == 3)
                        color = Color.SaddleBrown;
                    else if (i == 4 || i == 5)
                        color = Color.SkyBlue;
                    else if (i == 6 || i == 7)
                        color = Color.Orange;
                    new ExoDustGlow(VFXInstance.Position, pos, color, 0.2f, 0f, 18, 45).Spawn();
                }
                for (int i = 0; i < 4; i++)
                {
                    Vector2 firePos = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1f) * 300 + VFXInstance.Position;
                    Vector2 firevel = LAPUtilities.GetVector2(firePos, VFXInstance.Position) * 24;
                    ParticlePreset.NewGlowLozenge(firePos, firevel, Color.White, 30, 0.2f);
                }
            }
            if (VFXInstance.Time == 6)
                SpawnEnergy(VFXInstance.Position);
            if (VFXInstance.Time == 15)
            {
                SpawnSpark(VFXInstance.Position);
                new ShockWave(VFXInstance.Position, SpawnPointRot, Color.White, 25, 1.1f).Spawn();
                new CrossGlow(VFXInstance.Position, Vector2.Zero, Color.White, 25, 1f, 0.6f).Spawn();
                SpawnSmoke(VFXInstance.Position);
            }
            // 缩放核心辉光
            if (VFXInstance.Time < 40)
            {
                float progress = VFXInstance.Time / 40f;
                VFXInstance.Scale = MathF.Sin(progress * MathHelper.TwoPi) + 1f;
            }
        }
        public override void OnKill()
        {
        }
        public override void Draw()
        {
            if (VFXInstance.Time < 25)
            {
                LAPUtilities.ApplyDefaultShader();
                Texture2D lightpoint = LAPTextureRegister.LightPoint_NB.Value;
                Vector2 orig = lightpoint.Size() / 2;
                float rot = Main.GlobalTimeWrappedHourly;
                LAPUtilities.Draw(lightpoint, VFXInstance.Position - Main.screenPosition, null, Color.White, rot, orig, VFXInstance.Scale * 0.2f, 0, 0);
            }
        }
        public void SpawnEnergy(Vector2 Center)
        {
            Vector2 targetCent = Center;
            Vector2 spawnpos_G =  SpawnEnergyPoint[0];
            Vector2 firevel_G = LAPUtilities.GetVector2(spawnpos_G, targetCent).RotatedBy(-MathHelper.PiOver4 * 1.2f);
            ExoLine.Spawn(spawnpos_G, firevel_G, targetCent, Color.LimeGreen, 0.18f, 12f);

            Vector2 spawnpos2_G2 =   SpawnEnergyPoint[1];
            Vector2 firevel_G2 = LAPUtilities.GetVector2(spawnpos2_G2, targetCent).RotatedBy(-MathHelper.PiOver4 * 1.2f);
            ExoLine.Spawn(spawnpos2_G2, firevel_G2, targetCent, Color.LimeGreen, 0.18f, 12f);

            Vector2 spawnpos_br =   SpawnEnergyPoint[2];
            Vector2 firevel_br = LAPUtilities.GetVector2(spawnpos_br, targetCent).RotatedBy(-MathHelper.PiOver4 * 1.2f);
            ExoLine.Spawn(spawnpos_br, firevel_br, targetCent, Color.SaddleBrown, 0.16f, 12f);

            Vector2 spawnpos2_br2 =  SpawnEnergyPoint[3];
            Vector2 firevel_br2 = LAPUtilities.GetVector2(spawnpos2_br2, targetCent).RotatedBy(-MathHelper.PiOver4 * 1.2f);
            ExoLine.Spawn(spawnpos2_br2, firevel_br2, targetCent, Color.SaddleBrown, 0.16f, 12f);

            Vector2 spawnpos_O =   SpawnEnergyPoint[4];
            Vector2 firevel_O = LAPUtilities.GetVector2(spawnpos_O, targetCent).RotatedBy(-MathHelper.PiOver2);
            ExoTrail.Spawn(spawnpos_O, firevel_O, targetCent, Color.SkyBlue, 0.25f, 12f);

            Vector2 spawnpos2_O2 =  SpawnEnergyPoint[5];
            Vector2 firevel_O2 = LAPUtilities.GetVector2(spawnpos2_O2, targetCent).RotatedBy(-MathHelper.PiOver2);
            ExoTrail.Spawn(spawnpos2_O2, firevel_O2, targetCent, Color.SkyBlue, 0.25f, 12f);

            Vector2 spawnpos_b =   SpawnEnergyPoint[6];
            Vector2 firevel_b = LAPUtilities.GetVector2(spawnpos_b, targetCent).RotatedBy(-MathHelper.PiOver2);
            ExoTrail.Spawn(spawnpos_b, firevel_b, targetCent, Color.Orange, 0.22f, 12f);

            Vector2 spawnpos2_b2 =  SpawnEnergyPoint[7];
            Vector2 firevel_b2 = LAPUtilities.GetVector2(spawnpos2_b2, targetCent).RotatedBy(-MathHelper.PiOver2);
            ExoTrail.Spawn(spawnpos2_b2, firevel_b2, targetCent, Color.Orange, 0.22f, 12f);
        }
        public void SpawnSpark(Vector2 Center)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 firePos = Center;
                Vector2 firevel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 48 * Main.rand.NextFloat(0.6f, 1f);
                ParticlePreset.NewGlowLozenge(firePos, firevel, Color.White, 30, 0.3f);
            }
        }
        public void SpawnSmoke(Vector2 Center)
        {
            float spread = MathHelper.TwoPi / 25f;
            for (int i = 0; i < 25; i++)
            {
                float angle = spread * i;
                Vector2 firevel = Vector2.UnitX.RotatedBy(angle + SpawnPointRot) * Main.rand.NextFloat(0.6f, 1f);
                Color color = LAPUtilities.LerpColor(Color.Blue, new Color(57, 46, 115));
                CampFire.Spawn(Center, firevel * 20f, color, Main.rand.Next(30, 60), Main.rand.NextFloat(MathHelper.TwoPi), 0.75f, BlendStateID.NonPremult);
            }
        }
    }
}
