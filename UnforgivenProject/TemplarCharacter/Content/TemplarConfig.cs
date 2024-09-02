﻿using BepInEx.Configuration;
using TemplarMod.Modules;

namespace TemplarMod.Templar.Content
{
    public static class TemplarConfig
    {
        public static ConfigEntry<bool> forceUnlock;
        public static ConfigEntry<bool> noShieldVisual;
        public static void Init()
        {
            string section = "01 - General";
            string section2 = "02 - Stats";

            //add more here or else you're cringe
            forceUnlock = Config.BindAndOptions(
                section,
                "Unlock Unforgiven",
                false,
                "Unlock Unforgiven.", true);

            noShieldVisual = Config.BindAndOptions(
                section,
                "No Shield Visual",
                false,
                "Remove Wanderer's shield visual.", true);
        }
    }
}
