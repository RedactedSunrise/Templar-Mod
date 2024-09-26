using System;
using TemplarMod.Modules;
using TemplarMod.Templar;
using TemplarMod.Templar.Achievements;
using UnityEngine.UIElements;

namespace TemplarMod.Templar.Content
{
    public static class TemplarTokens
    {
        public static void Init()
        {
            AddUnforgivenTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Unforgiven.txt");
            //todo guide
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddUnforgivenTokens()
        {
            #region Unforgiven
            string prefix = TemplarSurvivor.TEMPLAR_PREFIX;

            string desc = "The Templar is a battle-hardened protector, serving holy justice with a slew of magical Auras- passively enchanting his allies and cursing his enemies.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Maximize your potential in fights by running headlong into crowds of menial enemies." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Play around the power of your passive auras by herding enemies into large crowds, rather than picking them off." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use Holy Bombardment and Willbreaker to keep airborne enemies on the ground, where you can reach them." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Activate Holy Cause before engaging with a large crowd of enemies to ensure you have first-strike advantage!" + Environment.NewLine + Environment.NewLine;

            string lore = "Lore.";
            string outro = "..and so he left, valor expressed and wounds licked.";
            string outroFailure = "..and so he vanished, fealty unproven.";
            
            Language.Add(prefix + "NAME", "Templar");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "First Line");
            Language.Add(prefix + "LORE", lore);
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_AURA_FIRE", "Cleansing Fire");
            Language.Add(prefix + "PASSIVE_DESCRIPTION_FIRE", $"After being in combat for 5 seconds, <style=cIsDamage>emanate an aura</style> that grants you and nearby allies <style=cIsDamage>Ablaze</style>. <style=cIsHealth>Radiant</style>.");
            Language.Add(prefix + "PASSIVE_AURA_CONVICT", "Conviction");
            Language.Add(prefix + "PASSIVE_DESCRIPTION_CONVICT", $"After being in combat for 5 seconds, <style=cIsDamage>emanate an aura</style> that makes any nearby enemy <style=cIsDamage>frail</style>. <style=cIsHealth>Radiant</style>.");
            Language.Add(prefix + "PASSIVE_AURA_PRAY", "Prayer");
            Language.Add(prefix + "PASSIVE_DESCRIPTION_PRAY", $"After being in combat for 5 seconds, <style=cIsDamage>emanate an aura</style> that grants you and nearby allies <style=cIsDamage>additional life over time</style>. <style=cIsHealth>Radiant</style>.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_SWING_NAME", "Zeal");
            Language.Add(prefix + "PRIMARY_SWING_DESCRIPTION", $"Swing in front dealing <style=cIsDamage>{TemplarStaticValues.swingDamageCoefficient * 100f}% damage</style>. You are grounded during the swing.");
            #endregion

            #region PrimaryAirborne
            Language.Add(prefix + "PRIMARY_SHOT_NAME", "Holy Bolt");
            Language.Add(prefix + "PRIMARY_SHOT_DESCRIPTION", "Shoot a bolt of holy energy for <style=cIsDamage>250% damage</style>. This skill is only available while airborne.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_STEEL_NAME", "Willbreaker");
            Language.Add(prefix + "SECONDARY_STEEL_DESCRIPTION", $"Grab enemies in front of you with holy light, dealing <style=cIsDamage>80% damage</style> on each struck target while pulling them to you.");
            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_SWEEP_NAME", "Holy Bombardment");
            Language.Add(prefix + "UTILITY_SWEEP_DESCRIPTION", $"Bombard the area with holy light for 6 seconds, dealing <style=cIsDamage>120% damage per second</style>. <style=cIsDamage>Plunging</style>. <style=cIsHealth>Radiant</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_HCAUSE_NAME", "Holy Cause");
            Language.Add(prefix + "SPECIAL_HCAUSE_DESCRIPTION", $"Force-activate your currently selected Aura, granting it 200% additional radius for 5 seconds.");

            Language.Add(prefix + "SPECIAL_SCEP_HCAUSE_NAME", "Radiance");
            Language.Add(prefix + "SPECIAL_SCEP_HCAUSE_DESCRIPTION", $"Dash towards an <style=cIsUtility>airborne</style> enemy then rapidly attack in an area for <style=cIsDamage>2x{TemplarStaticValues.specialFirstDamageCoefficient * 100f}% + {TemplarStaticValues.specialFinalDamageCoefficient * 100f}% damage</style>. " +
                $"Gain <style=cIsDamage>armor shred</style> for 6 seconds." + Tokens.ScepterDescription("<style=cIsUtility>Reset your secondary cooldown</style>. Enemies hit by <color=#FFBF66>First Breath</color> can be targetted by <color=#FFBF66>Sweeping Blade</color> again."));
            #endregion

            #region Achievements
            //Language.Add(Tokens.GetAchievementNameToken(UnforgivenMasterAchievement.identifier), "Unforgiven: Mastery");
            //Language.Add(Tokens.GetAchievementDescriptionToken(UnforgivenMasterAchievement.identifier), "As Unforgiven, beat the game or obliterate on Monsoon.");
            /*
            Language.Add(Tokens.GetAchievementNameToken(UnforgivenUnlockAchievement.identifier), "Dressed to Kill");
            Language.Add(Tokens.GetAchievementDescriptionToken(UnforgivenUnlockAchievement.identifier), "Get a Backstab.");
            */
            #endregion

            #endregion
        }
    }
}