using EntityStates;
using RoR2.Projectile;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using TemplarMod.Modules.BaseStates;
using TemplarMod.Templar.Content;

namespace TemplarMod.Templar.SkillStates
{
    public class HolyCause : BaseTemplarSkillState
    {
        public static float baseDuration = 0.2f;

        private float duration;
        public override void OnEnter()
        {
            RefreshState();

            base.OnEnter();

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(TemplarBuffs.AuraForceActivateBuff, 5f);
            }

            this.duration = baseDuration / this.attackSpeedStat;

            base.characterBody.SetAimTimer(1f);

            base.PlayAnimation("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration);

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
            return InterruptPriority.Any;
        }
    }
}
