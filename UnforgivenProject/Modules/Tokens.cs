using TemplarMod.Templar.Content;

namespace TemplarMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string radiantKeyword = KeywordText("Radiant", "Gains 1 effective meter of radius per <style=cIsHealth>5 armor</style>.");

        public static string plungingKeyword = KeywordText("Plunging", "Struck airborne enemies are pushed to the ground, while grounded enemies are rooted.");

        public static string aflameKeyword = KeywordText("Aflame", "Those affected burn enemies for additional <style=cIsDamage>damage over time</style>.");

        public static string frailKeyword = KeywordText("Frail", "Afflicted enemies deal <style=cIsHealth>less damage</style> and <style=cIsDamage>take more</style>.");

        public static string rejuvenatingKeyword = KeywordText("Rejuvenating", "Those affected regenerate an additional <style=cIsHealing>1.5</style> health per second.");

        public static string groundedKeyword = KeywordText("Grounding", "The swung weapon is so heavy that it's user <style=cIsDamage>cannot jump</style> while swinging it.");

        public static string slayerKeyword = KeywordText("Slayer", "The skill deals 2% more damage per 1% of health the target has lost, up to <style=cIsDamage>3x</style> damage.");

        public static string unforgivenSwiftKeyword = KeywordText("Swift", "The skill has a lower <style=cIsUtility>cooldown</style> based on your <style=cIsDamage>attack speed</style>.");
        public static string DamageText(string text)
        {
            return $"<style=cIsDamage>{text}</style>";
        }
        public static string DamageValueText(float value)
        {
            return $"<style=cIsDamage>{value * 100}% damage</style>";
        }
        public static string UtilityText(string text)
        {
            return $"<style=cIsUtility>{text}</style>";
        }
        public static string RedText(string text) => HealthText(text);

        public static string GreenText(string text) => HealingText(text);

        public static string HealingText(string text)
        {
            return $"<style=cIsHealing>{text}</style>";
        }
        public static string HealthText(string text)
        {
            return $"<style=cIsHealth>{text}</style>";
        }
        public static string KeywordText(string keyword, string sub)
        {
            return $"<style=cKeywordName>{keyword}</style><style=cSub>{sub}</style>";
        }
        public static string ScepterDescription(string desc)
        {
            return $"\n<color=#d299ff>SCEPTER: {desc}</color>";
        }

        public static string GetAchievementNameToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_NAME";
        }
        public static string GetAchievementDescriptionToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_DESCRIPTION";
        }
    }
}