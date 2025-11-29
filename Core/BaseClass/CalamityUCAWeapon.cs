using CalamityMod;
using CalamityMod.Items.Weapons.Melee;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    /// <summary>
    /// 这个抽象类用于直接用最懒惰的方式创建一个武器
    /// 只需要写入对应的ClassType，这个类就会自动在SSD设定旅途模式研究数、SD里写入伤害类型，并分配好本地化分类
    /// 本质上也是自建轮子，但还好
    /// </summary>
    public abstract class CalamityUCAWeapon: ModItem, ILocalizedModType
    {
        public enum ClassType
        {
            Melee,
            Ranged,
            Magic,
            Summon,
            Rogue
        };
        /// <summary>
        /// 这是一个枚举类型
        /// </summary>
        public virtual ClassType WeaponType {  get; }
        public new string LocalizationCategory => $"Weapons.{WeaponType}";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ExSSD();
        }
        public virtual void ExSSD() { }
        public override void SetDefaults()
        {
            Item.DamageType = GetNeedDamageClass;
            ExSD();
        }
        public virtual void ExSD() { }
        private DamageClass GetNeedDamageClass
        {
            get
            {
                return WeaponType switch
                {
                    ClassType.Melee => DamageClass.Melee,
                    ClassType.Ranged => DamageClass.Ranged,
                    ClassType.Magic => DamageClass.Magic,
                    ClassType.Summon => DamageClass.Summon,
                    ClassType.Rogue => ModContent.GetInstance<RogueDamageClass>(),
                    _ => DamageClass.Generic,
                };
            }
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = GetNeedItemGroup;
        }
        private ContentSamples.CreativeHelper.ItemGroup GetNeedItemGroup
        {
            get
            {
                return WeaponType switch
                {
                    ClassType.Melee => ContentSamples.CreativeHelper.ItemGroup.MeleeWeapon,
                    ClassType.Ranged => ContentSamples.CreativeHelper.ItemGroup.RangedWeapon,
                    ClassType.Magic => ContentSamples.CreativeHelper.ItemGroup.MagicWeapon,
                    ClassType.Summon=> ContentSamples.CreativeHelper.ItemGroup.SummonWeapon,
                    _ => ContentSamples.CreativeHelper.ItemGroup.RemainingUseItems
                };
            }
        }
    }
}
