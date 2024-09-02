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
    public class HolyBarrage : BaseTemplarSkillState
    {
        public static GameObject areaIndicatorPrefab = TemplarAssets.holyBarrageIndicatorPrefab;

        public static GameObject muzzleflashEffect = TemplarAssets.holyBarrageMuzzleFlash;

        // public static GameObject goodCrosshairPrefab;

        // public static GameObject badCrosshairPrefab;

        // public static string prepFistSoundString;

        // public static string endFistSoundString;

        // public static string startTargetingLoopSoundString;

        // public static string stopTargetingLoopSoundString;

        public static float maxDistance = 600;

        // public static string fireSoundString;

        public static float maxSlopeAngle = 180;

        //public static GameObject initialEffect;

        public static GameObject muzzleFlashEffect = TemplarAssets.holyBarrageMuzzleFlash;

        public static GameObject barrageProjectilePrefab = TemplarAssets.holyBarragePrefab;

        public static float baseDuration = 0.4f;
        public static float barrageDamageCoefficient = 1.2f;
        public static float barrageForce = 0f;

        // public static float fuseOverride;

        public static int abilityAimType;

        // public static float cameraTransitionOutTime;

        // public static float smallHopVelocity;

        private float duration;

        // private float stopwatch;

        private bool goodPlacement;

        private GameObject areaIndicatorInstance;

        // private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        protected CameraTargetParams.AimRequest aimRequest;

        private bool cameraStarted;


        // Playing the attack animation on player & updating the area indicator.


        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            base.characterBody.SetAimTimer(duration + 2f);
            base.PlayAnimation("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration);
            // Util.PlaySound(prepFistSoundString, base.gameObject);
            // Util.PlaySound(startTargetingLoopSoundString, base.gameObject);
            areaIndicatorInstance = UnityEngine.Object.Instantiate(areaIndicatorPrefab);
            UpdateAreaIndicator();
        }


        // Create Reticle & Update Reticle whether placement is good or bad.


        private void UpdateAreaIndicator()
        {
            bool flag = goodPlacement;
            goodPlacement = false;
            areaIndicatorInstance.SetActive(value: true);
            if ((bool)areaIndicatorInstance)
            {
                float num = maxDistance;
                float extraRaycastDistance = 0f;
                if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(GetAimRay(), base.gameObject, out extraRaycastDistance), out var hitInfo, num + extraRaycastDistance, LayerIndex.world.mask))
                {
                    areaIndicatorInstance.transform.position = hitInfo.point;
                    goodPlacement = Vector3.Angle(Vector3.up, hitInfo.normal) < maxSlopeAngle;
                }
                /* if (flag != goodPlacement || crosshairOverrideRequest == null)
                 {
                     crosshairOverrideRequest?.Dispose();
                     GameObject crosshairPrefab = (goodPlacement ? goodCrosshairPrefab : badCrosshairPrefab);
                     crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairPrefab, CrosshairUtils.OverridePriority.Skill);
                 }
                 */
            }
            areaIndicatorInstance.SetActive(goodPlacement);
        }

        // I don't think I fully understood what this means (ask again after overview of full codebase)

        public override void Update()
        {
            base.Update();
            UpdateAreaIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!cameraStarted && base.fixedAge > 0.1f)
            {
                if ((bool)base.cameraTargetParams)
                {
                  aimRequest = base.cameraTargetParams.RequestAimType((CameraTargetParams.AimType)abilityAimType);
                }
                cameraStarted = true;
            }
            if (base.inputBank.skill3.down || !base.isAuthority)
            {
                return;
            }
            if (goodPlacement)
            {
                base.PlayAnimation("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration);
                base.PlayAnimation("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration);
                // Util.PlaySound(fireSoundString, base.gameObject);
                if ((bool)areaIndicatorInstance)
                {
                    EffectManager.SpawnEffect(muzzleFlashEffect, new EffectData
                    {
                        origin = areaIndicatorInstance.transform.position
                    }, transmit: true);
                    if (base.isAuthority)
                    {
                        //EffectManager.SimpleMuzzleFlash(muzzleflashEffect, base.gameObject, "MuzzleLeft", transmit: true);
                        //EffectManager.SimpleMuzzleFlash(muzzleflashEffect, base.gameObject, "MuzzleRight", transmit: true);
                        Util.CheckRoll(critStat, base.characterBody.master);
                        FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
                        fireProjectileInfo.projectilePrefab = barrageProjectilePrefab;
                        fireProjectileInfo.position = areaIndicatorInstance.transform.position;
                        fireProjectileInfo.rotation = Quaternion.identity;
                        fireProjectileInfo.owner = base.gameObject;
                        fireProjectileInfo.damage = damageStat * barrageDamageCoefficient;
                        fireProjectileInfo.damageTypeOverride = DamageType.Stun1s;
                        fireProjectileInfo.force = barrageForce;
                        fireProjectileInfo.crit = base.characterBody.RollCrit();
                        ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                    }
                }
            }
            else
            {
                base.skillLocator.utility.AddOneStock();
            }
            outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            // Util.PlaySound(endFistSoundString, base.gameObject);
            // Util.PlaySound(stopTargetingLoopSoundString, base.gameObject);
            EntityState.Destroy(areaIndicatorInstance.gameObject);
            // crosshairOverrideRequest?.Dispose();

            //Comment this out please
            //characterBody.AddBuff(TemplarBuffs.AflameBuff);
            //-


            aimRequest?.Dispose();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}