using TemplarMod.Modules.BaseStates;
using TemplarMod.Templar.SkillStates;

namespace TemplarMod.Templar.Content
{
    public static class TemplarStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(BaseTemplarSkillState));
            Modules.Content.AddEntityState(typeof(BaseUnforgivenState));
            Modules.Content.AddEntityState(typeof(MainState));

            Modules.Content.AddEntityState(typeof(SlashCombo));
            Modules.Content.AddEntityState(typeof(ProjectileHolyPierce));
            Modules.Content.AddEntityState(typeof(HolyBarrage));
            Modules.Content.AddEntityState(typeof(ChainPull));
        }
    }
}
