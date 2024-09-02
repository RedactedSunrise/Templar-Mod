using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using RoR2.Projectile;
using RoR2.UI;
using System;
using System.Text;
using TemplarMod.Modules.BaseStates;
using TemplarMod.Templar.Content;

namespace TemplarMod.Templar.SkillStates
{
    public class ChainPull : BaseTemplarSkillState
    {
        // SerializeField]
        // public string sound;

        //[SerializeField]
        //public string muzzle;

        // [SerializeField]
        // public GameObject fireEffectPrefab; 

        [SerializeField]
        public float baseDuration = 0.4f;
        [SerializeField]
        public float fieldOfView = 60f;
        [SerializeField]
        public float maxDistance = 40f;
        [SerializeField]
        public float chainpullDamageCoefficient = 0.8f;
        [SerializeField]
        public float chainpullProcCoefficient = 0.6f;

        [SerializeField]
        public float idealDistanceToPlaceTargets = 50f;

        [SerializeField]
        public float liftVelocity = 10f;

        [SerializeField]
        public float slowDuration = 2f;

        [SerializeField]
        public float groundKnockbackDistance = 20f;

        [SerializeField]
        public float airKnockbackDistance = 20f;

        //public static AnimationCurve shoveSuitabilityCurve;

        private float duration = 0.6f;

        // private static int FireSonicBoomStateHash = Animator.StringToHash("FireSonicBoom");

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            base.PlayAnimation("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration);
            //Util.PlaySound(sound, base.gameObject);
            Ray aimRay = GetAimRay();
            aimRay.origin -= aimRay.direction;
            if (NetworkServer.active)
            {
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.teamMaskFilter = TeamMask.all;
                bullseyeSearch.maxAngleFilter = fieldOfView * 0.5f;
                bullseyeSearch.maxDistanceFilter = maxDistance;
                bullseyeSearch.searchOrigin = aimRay.origin;
                bullseyeSearch.searchDirection = aimRay.direction;
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
                bullseyeSearch.filterByLoS = false;
                bullseyeSearch.RefreshCandidates();
                bullseyeSearch.FilterOutGameObject(base.gameObject);
                IEnumerable<HurtBox> enumerable = bullseyeSearch.GetResults().Where(Util.IsValid).Distinct(default(HurtBox.EntityEqualityComparer));
                TeamIndex team = GetTeam();
                foreach (HurtBox item in enumerable)
                {
                    if (FriendlyFireManager.ShouldSplashHitProceed(item.healthComponent, team))
                    {
                        Vector3 vector = item.transform.position - aimRay.origin;
                        float magnitude = vector.magnitude;
                        _ = new Vector2(vector.x, vector.z).magnitude;
                        Vector3 vector2 = vector / magnitude;
                        float num = 1f;
                        CharacterBody body = item.healthComponent.body;
                        if ((bool)body.characterMotor)
                        {
                            num = body.characterMotor.mass;
                        }
                        else if ((bool)item.healthComponent.GetComponent<Rigidbody>())
                        {
                            num = base.rigidbody.mass;
                        }
                        float num2 = 1f;
                        body.RecalculateStats();
                        float acceleration = body.acceleration;
                        Vector3 vector3 = vector2;
                        float num3 = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(idealDistanceToPlaceTargets - magnitude), 0f - acceleration) * Mathf.Sign(idealDistanceToPlaceTargets - magnitude);
                        vector3 *= num3;
                        vector3.y = liftVelocity;
                        DamageInfo damageInfo = new DamageInfo
                        {
                            attacker = base.gameObject,
                            damage = this.damageStat * chainpullDamageCoefficient,
                            damageType = DamageType.Generic,
                            position = item.transform.position,
                            procCoefficient = chainpullProcCoefficient,
                        };
                        item.healthComponent.TakeDamageForce(vector3 * (num * num2 * -1), alwaysApply: true, disableAirControlUntilCollision: true);
                        item.healthComponent.TakeDamage(damageInfo);
                        GlobalEventManager.instance.OnHitEnemy(damageInfo, item.healthComponent.gameObject);
                        GlobalEventManager.instance.OnHitAll(damageInfo, item.healthComponent.gameObject);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}