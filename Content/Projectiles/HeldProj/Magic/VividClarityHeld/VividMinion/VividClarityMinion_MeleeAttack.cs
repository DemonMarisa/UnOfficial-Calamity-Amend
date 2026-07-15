using LAP.Assets.Sounds;
using LAP.Core.Graphics.VFX;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.VFXs;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public partial class VividClaritySupportMinion
    {
        public VFXInstance GreatSword;
        public Vector2 DashBegin;
        public Vector2 DashEnd;
        public int DashCount;
        public ref float VGSScale => ref GreatSword.AiFloat[0];
        public ref float VGSDrawScale => ref GreatSword.AiFloat[1];
        public void UpdateMeleeAttack()
        {
            if (GreatSword is null)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.MagicCharge_ER, Projectile.Center);
                Projectile.velocity = Vector2.Zero;
                AttackTimer = 0;
                GreatSword = VividGreatSword.Spawn(Projectile.whoAmI);
                VGSScale = 0.3f;
                GreatSword.AiBool[0] = true;
                GreatSword.AiBool[1] = true;
                DashBegin = Projectile.Center;
            }
            NPC npc = LAPUtilities.FindClosestTarget(Owner.Center, 3000);
            if (AttackTimer < 25)
            {
                if (AttackTimer == 1)
                    DashEnd = DashBegin - Projectile.rotation.ToRotationVector2() * 200;
                Projectile.Center = Vector2.Lerp(DashBegin, DashEnd, EasingHelper.EaseOutCubic(AttackTimer / 25f));
                if (npc is not null)
                    Projectile.rotation = Utils.AngleTowards(Projectile.rotation, LAPUtilities.GetVector2(Projectile.Center, npc.Center).ToRotation(), 0.25f);
            }
            else if (AttackTimer >= 25 && AttackTimer < 35)
            {
                if (AttackTimer == 25)
                {
                    DashCount++;
                    SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordUse with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                    DashBegin = DashEnd; 
                    DashEnd = DashEnd + Projectile.rotation.ToRotationVector2() * 1600;
                    if (Projectile.IsLocalPlayer())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2(), Projectile.rotation.ToRotationVector2(), ProjectileType<ExoSlash>(), Projectile.damage * 15, Projectile.knockBack, Projectile.owner);
                    }
                }
                Projectile.Center = Vector2.Lerp(DashBegin, DashEnd, BezierEaseHelper.BezierSmooth(Vector2.UnitY, Vector2.UnitY,(AttackTimer - 25) / 15f));
            }
            else if (AttackTimer >= 35 && AttackTimer < (DashCount < 2 ?  60 : 55))
            {
                if (AttackTimer == 35)
                {
                    // 注意，这里开始淡出就用插值进行缩放了，大约需要23帧才会完全淡出
                    // 如果这里太长，那么就会导致无限重置冲刺，因为淡出后大剑就会判定为null，执行第一轮的重设
                    FadeOutBlade = true;
                }
                if (npc is not null)
                {
                    Projectile.rotation = Utils.AngleTowards(Projectile.rotation, LAPUtilities.GetVector2(Projectile.Center, npc.Center).ToRotation(), 0.18f);
                }
                else
                    ChangeState(Idle);
            }
            else
            {
                DashCount = 0;
                if (npc is not null)
                {
                    ChangeState(RangedAttack);
                }
                else
                    ChangeState(Idle);
            }
        }
    }
}
