using EntityStates;
using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using TemplarMod.Modules.BaseStates;
using TemplarMod.Templar.Content;

namespace TemplarMod.Templar.SkillStates
{
    public class ProjectileHolyPierce : BaseTemplarSkillState
    {
        public static float damageCoefficient = TemplarStaticValues.tornadoDamageCoefficient;
        public static float procCoefficient = 1.0f;
        public static float baseDuration = 0.5f;
        public static float throwForce = 250f;

        public GameObject holyBolt = TemplarAssets.holyboltPrefab;

        private float duration;
        public override void OnEnter()
        {
            RefreshState();

            base.OnEnter();

            this.duration = baseDuration / this.attackSpeedStat;

            base.characterBody.SetAimTimer(2f);

            base.PlayAnimation("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration);

            if (NetworkServer.active) base.characterBody.ClearTimedBuffs(TemplarBuffs.stabMaxStacksBuff);

            this.Fire();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            Util.PlaySound("sfx_Templar_throw_nado", base.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                {
                    projectilePrefab = holyBolt,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    owner = base.gameObject,
                    damage = damageCoefficient * this.damageStat,
                    force = 400f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = throwForce,
                    useSpeedOverride = false,
                    damageTypeOverride = DamageType.Generic
                });

            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
