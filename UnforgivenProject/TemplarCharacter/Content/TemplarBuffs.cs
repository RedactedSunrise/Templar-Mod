using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TemplarMod.Templar.Content
{
    public static class TemplarBuffs
    {
        public static BuffDef stabStackingBuff;
        public static BuffDef stabMaxStacksBuff;
        public static BuffDef dashCooldownBuff;
        public static BuffDef stackingDashDamageBuff;
        public static BuffDef specialSlamTrackerBuff;
        public static BuffDef airborneBuff;
        public static BuffDef lastBreathBuff;
        public static BuffDef TemplarGroundedBuff;
        public static BuffDef AflameBuff;
        public static BuffDef AuraTimerBuff;
        public static BuffDef AuraActiveBuff;

        public static void Init(AssetBundle assetBundle)
        {
            stabStackingBuff = Modules.Content.CreateAndAddBuff("UnforgivenStackingBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/CritOnUse/texBuffFullCritIcon.tif").WaitForCompletion(),
                Color.gray, true, false, false);

            stabMaxStacksBuff = Modules.Content.CreateAndAddBuff("UnforgivenMaxStacksBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/CritOnUse/texBuffFullCritIcon.tif").WaitForCompletion(),
                TemplarAssets.templarColor, false, false, false);

            dashCooldownBuff = Modules.Content.CreateAndAddBuff("UnforgivenDashCooldown", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texMovespeedBuffIcon.tif").WaitForCompletion(),
                Color.gray, false, false, true);

            stackingDashDamageBuff = Modules.Content.CreateAndAddBuff("UnforgivenStackingDamageBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/AttackSpeedOnCrit/texBuffAttackSpeedOnCritIcon.tif").WaitForCompletion(),
                TemplarAssets.templarColor, true, false, false);

            specialSlamTrackerBuff = Modules.Content.CreateAndAddBuff("UnforgivenSlamTrackerBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texBuffSlow50Icon.tif").WaitForCompletion(),
                TemplarAssets.templarColor, false, false, false);

            airborneBuff = Modules.Content.CreateAndAddBuff("UnforgivenAirborneTrackerBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/EliteLunar/texBuffAffixLunar.tif").WaitForCompletion(),
                TemplarAssets.templarColor, false, false, false);

            lastBreathBuff = Modules.Content.CreateAndAddBuff("LastBreathBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/LunarSkillReplacements/texBuffLunarDetonatorIcon.tif").WaitForCompletion(),
                TemplarAssets.templarColor, false, false, false);

            TemplarGroundedBuff = Modules.Content.CreateAndAddBuff("TemplarGroundedBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texBuffSlow50Icon.tif").WaitForCompletion(),
                Color.grey, false, false, false);
            AflameBuff = Modules.Content.CreateAndAddBuff("Aflame", Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/StrengthenBurn/texBuffStrongerBurnIcon.tif").WaitForCompletion(),
                Color.green, false, false, false);

            AuraTimerBuff = Modules.Content.CreateAndAddBuff("AuraTimer", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/WardOnLevel/texBuffWarbannerIcon.tif").WaitForCompletion(),
                Color.white, false, true, true);
            AuraActiveBuff = Modules.Content.CreateAndAddBuff("AuraTimer", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/WardOnLevel/texBuffWarbannerIcon.tif").WaitForCompletion(),
                Color.red, false, false, false);

        }
    }
}
