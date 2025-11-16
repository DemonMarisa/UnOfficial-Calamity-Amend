using CalamityMod;
using CalamityMod.Balancing;
using CalamityMod.Tiles.FurnitureMonolith;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Projectiles.HeldProj;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        // 外围的玩家伤害减免
        public bool HeldNightShield = false;
        public bool WeakHeldNightShield = false;
        public static int NightShieldMaxHP = 400;
        public int NightShieldHP = 400;
        public bool NightShieldCanDefense = true;

        public static int MaxTerraRayRestore = 3;
        public int TerraRayRestore = 3;
        public bool TerraRestore = false;
        public static int TerraRayChargeCD = 2700;
        public int TerraRayCharge = 0;
        public int TerraRayHealCD = 0;
        public int TerraRayUseSkillCount = 0;

        public int ElementalRayStates = ElementalRayState.Misc;

        public int SoulPiercerSGSUse = 0;

        public static int MaxShadowPlayer = 12;
        public static bool CanShadowDoge = false;
        public bool _anyHammerStriking;
        //一个EModPlayer塞tm3000行我都不知道我的内容塞也太他妈绝望了
        //反正这里专门处理锤子的各种有的没的特殊情况
        //分别用到钩子都会有注释专门写明白
        #region 盗贼潜伏锤处理
        //当前场上是否有盗贼挂载锤
        public bool _anyHammerAttacking = false;
        public int _cacheHammer = -1;
        public int _cacheHeadType = -1;
        public int _cacheBodyType = -1;
        public int _cacheLegsType = -1;
        public bool ShouldHandleHammerStealth = false;
        public bool CanDisableGuideForGodsHammer = false;
        public bool CanDisableGuideForGrandHammer = false;
        public bool ShouldGiveSpareGodsHammer = false;
        /// <summary>
        /// 在ResetEffect内处理手持锤子的效果，包括不限于玩家是否有挂载中的锤子，手持启用潜伏条等
        /// </summary>
        private void RogueHammerReset()
        {
            var calPlayer = Player.Calamity();
            _anyHammerAttacking = false;

            //下方的效果对于大锤子全部不生效
            int heldType = Player.HeldItem.type;
            //缓存的锤子也会算进去，因为这里需要处理从锤子切入到其他盗贼武器时重置潜伏条的情况
            if (RogueHammer.Contains(heldType) || RogueHammer.Contains(_cacheHammer))
            {
                //所有锤子启用潜伏条，潜伏值减半
                calPlayer.wearingRogueArmor = true;
                calPlayer.stealthStrikeHalfCost = true;
                //锤子常驻10潜伏值，这个效果不再有任何条件制约
                calPlayer.rogueStealthMax += BaseHammerItem.BaseMaxStealth;
                ShouldHandleHammerStealth = true;
                //全体锤子无法生效任何形式的潜伏增伤
                //下方的代码从灾厄那复制
                DisabelStealthDamageBoost();
            }
            else
                ShouldHandleHammerStealth = false;
        }
        public List<int> RogueHammer =
            [
                ModContent.ItemType<PunishmentHammer>(),
                ModContent.ItemType<BlazingHammer>(),
                ModContent.ItemType<NightmareHammer>(),
                ModContent.ItemType<DivineHammer>(),
                //ModContent.ItemType<ThunderHammer>(),
            ];
        private void DisabelStealthDamageBoost()
        {
            var calPlayer = Player.Calamity();
            double averagedStealthGen = 0.8 * calPlayer.stealthGenMoving + 0.2 * calPlayer.stealthGenStandstill;
            double fakeStealthTime = 4f / averagedStealthGen;
            int realUseTime = Math.Max(Player.HeldItem.useTime, Player.HeldItem.useAnimation);
            double useTimeFactor = 0.75 + 0.75 * Math.Log(realUseTime + 2D, 4D);
            double stealthGenFactor = Math.Max(Math.Pow(fakeStealthTime, 2D / 3D), 1.5);
            calPlayer.stealthDamage -= (float)(calPlayer.rogueStealth * BalancingConstants.UniversalStealthStrikeDamageFactor * useTimeFactor * stealthGenFactor);
        }
        //存储。
        private void HammerTagSave(TagCompound tag)
        {
            tag.Add(nameof(CanDisableGuideForGodsHammer), CanDisableGuideForGodsHammer);
            tag.Add(nameof(CanDisableGuideForGrandHammer), CanDisableGuideForGrandHammer);
            tag.Add(nameof(ShouldGiveSpareGodsHammer), ShouldGiveSpareGodsHammer);
        }

        private void HammerTagLoad(TagCompound tag)
        {
            CanDisableGuideForGodsHammer = tag.GetBool(nameof(CanDisableGuideForGodsHammer));
            CanDisableGuideForGrandHammer = tag.GetBool(nameof(CanDisableGuideForGrandHammer));
            ShouldGiveSpareGodsHammer = tag.GetBool(nameof(ShouldGiveSpareGodsHammer));
        }
        /// <summary>
        /// 梦魇锤投掷微光转为弑神锤的引导
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool StopGodHammerShimemrGuide(Item item)
        {
            if (item.type == ModContent.ItemType<DivineHammer>() && DownedBossSystem.downedDoG)
            {
                CanDisableGuideForGodsHammer = true;
                return true;
            }
            if (item.type == ModContent.ItemType<ThunderHammer>() && DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs)
            {
                CanDisableGuideForGrandHammer = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 在MiscEffect内书写的专门处理锤子重置方式。
        /// </summary>
        private void RefreshAllHammerStateOnNeed()
        {
            DisabelStealthDamageBoost();
            //玩家复活后，给予弑神锤本身
            //if (ShouldGiveSpareGodsHammer)
            //{
            //    Player.QuickSpawnItem(Player.GetSource_FromThis(), ModContent.ItemType<DivineHammer>());
            //    ShouldGiveSpareGodsHammer = false;
            //}
            ////大型锤子特供。在悬空的锤子存续时
            //if (Player.ownedProjectileCounts[ModContent.ProjectileType<ThunderHandler>()] > 0)
            //{
            //    //启动无限飞行
            //    Player.Calamity().infiniteFlight = true;
            //    //获得原版翱翔证章的所有数值
            //    Player.runAcceleration *= 1.75f;
            //    Player.jumpSpeedBoost += 1.8f;
            //}

            //只有锤子才会重置潜伏条
            int heldType = Player.HeldItem.type;
            if (!ShouldHandleHammerStealth)
                return;
            //无论什么情况下，切换至锤子时都强行重置一次潜伏条
            if (heldType != _cacheHammer)
            {
                Player.Calamity().rogueStealth = 0f;
                _cacheHammer = heldType;
            }
            //特殊情况：在切装的情况下判定
            //但凡有一件有不同就干掉潜伏条
            if (Player.armor[0].type != _cacheHeadType || Player.armor[1].type != _cacheBodyType || Player.armor[2].type != _cacheLegsType)
            {
                Player.Calamity().rogueStealth = 0f;
                _cacheHeadType = Player.armor[0].type;
                _cacheBodyType = Player.armor[1].type;
                _cacheLegsType = Player.armor[2].type;
            }
        }
        #endregion
        public override bool OnPickup(Item item)
        {
            if (StopGodHammerShimemrGuide(item))
                return true;
            return base.OnPickup(item);
        }

        public override void ResetEffects()
        {
            if (NightShieldHP > NightShieldMaxHP)
                NightShieldHP = NightShieldMaxHP;

            if (NightShieldHP < 0)
                NightShieldHP = 0;

            // 充满后才会激活护盾
            if (NightShieldHP >= NightShieldMaxHP)
                NightShieldCanDefense = true;

            // 如果护盾归零则失效，必须充满才可以抵挡伤害
            if (NightShieldHP <= 0)
                NightShieldCanDefense = false;

            UpdateTerraRayCD();
            RogueHammerReset();
        }
        public void UpdateTerraRayCD()
        {
            // 存储与更新回血次数限制
            if (TerraRayRestore > MaxTerraRayRestore)
                TerraRayRestore = MaxTerraRayRestore;
            if (TerraRayRestore < 0)
                TerraRayRestore = 0;
            if (TerraRayRestore < MaxTerraRayRestore && TerraRayCharge < TerraRayChargeCD)
            {
                TerraRayCharge++;
            }
            if (TerraRayCharge >= TerraRayChargeCD)
            {
                TerraRayCharge = 0;
                TerraRayRestore++;
            }
            // 吸血CD
            if (TerraRayHealCD > 0)
                TerraRayHealCD--;
            // 控制免伤
            if (TerraRayUseSkillCount > 0)
                TerraRayUseSkillCount--;
            // 控制灵魂巨剑的免伤
            if (SoulPiercerSGSUse > 0)
                SoulPiercerSGSUse--;
        }
        public override void LoadData(TagCompound tag)
        {
            HammerTagLoad(tag);
        }
        public override void SaveData(TagCompound tag)
        {
            HammerTagSave(tag);
        }
        public override void PostUpdateEquips()
        {
        }
        public override void PostUpdateMiscEffects()
        {
            AddNightBoost();
            AddCarnageBoost();
            AddTerraBoost();
            AddElementalBoost();
            AddShadowBoltStaffBoost();
            RefreshAllHammerStateOnNeed();
        }

        public override void PostUpdate()
        {
            // 在最后一帧重置，这样就可以延迟一帧取到效果
            HeldNightShield = false;
            WeakHeldNightShield = false;
            if (TerraRestore)
            {
                Player.Heal(Player.statLifeMax2 / 4);
                TerraRestore = false;
            }
        }
    }
}
