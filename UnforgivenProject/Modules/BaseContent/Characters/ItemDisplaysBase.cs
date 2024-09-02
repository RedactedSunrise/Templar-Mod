using RoR2;
using TemplarMod.Modules;
using System.Collections.Generic;

namespace TemplarMod.Modules.Characters
{
    public abstract class ItemDisplaysBase
    {
        public void SetItemDisplays(ItemDisplayRuleSet itemDisplayRuleSet)
        {
            List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            ItemDisplays.LazyInit();

            SetItemDisplayRules(itemDisplayRules);

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();

            ItemDisplays.DisposeWhenDone();
        }

        protected abstract void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules);
    }
}