using RoR2;
using TemplarMod.Modules.Achievements;
using TemplarMod.Templar;

namespace TemplarMod.Templar.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 0)]
    public class UnforgivenMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = TemplarSurvivor.TEMPLAR_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = TemplarSurvivor.TEMPLAR_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => TemplarSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}