using UnityEngine;
using EntityStates;
using TemplarMod.Modules.BaseStates;
using RoR2;
using UnityEngine.AddressableAssets;
using TemplarMod.Templar.Content;
using static R2API.DamageAPI;
using UnityEngine.Networking;
using R2API.Networking;
using TemplarMod.Templar.Components;
using R2API.Networking.Interfaces;

namespace TemplarMod.Templar.SkillStates
{
    public class SlashCombo : BaseMeleeAttack
    {
        protected GameObject swingEffectInstance;
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "MeleeHitbox";

            damageType = DamageType.Generic;
            damageCoefficient = TemplarStaticValues.swingDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 700f;
            bonusForce = Vector3.zero;
            baseDuration = 1.5f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.4f;
            attackEndPercentTime = 0.8f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 1.0f;

            hitStopDuration = 0.25f;
            attackRecoil = 2f / attackSpeedStat;
            hitHopVelocity = 0f;

            swingSoundString = EntityStates.Merc.Weapon.GroundLight2.slash1Sound;
            hitSoundString = "";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = TemplarAssets.swordSwingEffect;
            hitEffectPrefab = TemplarAssets.unforgivenHitEffect;

            impactSound = TemplarAssets.swordImpactSoundEvent.index;

            base.OnEnter();

            if (NetworkServer.active)
            {
                characterBody.AddBuff(TemplarBuffs.TemplarGroundedBuff);
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("MeleePivot").rotation = Util.QuaternionSafeLookRotation(direction);
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectInstance = Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(TemplarBuffs.TemplarGroundedBuff);
            }

            if (this.swingEffectInstance) EntityState.Destroy(this.swingEffectInstance);
        }


    }
} 