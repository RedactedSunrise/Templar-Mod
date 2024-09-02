using RoR2;
using UnityEngine;
using TemplarMod.Templar;
using TemplarMod.Templar.Achievements;

namespace TemplarMod.Templar.Content
{
    public static class TemplarUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef ascendencySkinUnlockableDef = null;

        public static void Init()
        {
            ascendencySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(UnforgivenMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(UnforgivenMasteryAchievement.unlockableIdentifier),
                TemplarSurvivor.instance.assetBundle.LoadAsset<Sprite>("texWhirlwindSkin"));
            /*
            if (true == false)
            {
                characterUnlockableDef = Modules.Content.CreateAndAddUnlockableDef(UnforgivenUnlockAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(UnforgivenUnlockAchievement.unlockableIdentifier),
                UnforgivenSurvivor.instance.assetBundle.LoadAsset<Sprite>("texUnforgivenIcon"));
            }
            */
        }
    }
}
