using HG;
using Newtonsoft.Json.Linq;
using R2API;
using RoR2;
using RoR2.Projectile;
using TemplarMod.Modules;
using TemplarMod.Templar.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static RoR2.DotController;

namespace TemplarMod.Templar.Content
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType Default;
        public static DamageAPI.ModdedDamageType KnockAirborne;
        public static DamageAPI.ModdedDamageType ResetDashes;

        internal static void Init()
        {
            Default = DamageAPI.ReserveDamageType();
            KnockAirborne = DamageAPI.ReserveDamageType();
            ResetDashes = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }
        private static void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            DamageInfo damageInfo = damageReport.damageInfo;
            if (!damageReport.attackerBody || !damageReport.victimBody)
            {
                return;
            }
            HealthComponent victim = damageReport.victim;
            GameObject inflictorObject = damageInfo.inflictor;
            CharacterBody victimBody = damageReport.victimBody;
            EntityStateMachine victimMachine = victimBody.GetComponent<EntityStateMachine>();
            CharacterBody attackerBody = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            TemplarController iController = attackerBody.GetComponent<TemplarController>();
            if (NetworkServer.active)
            {
            }
        }
    }
}
