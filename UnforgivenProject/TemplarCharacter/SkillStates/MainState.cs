﻿using UnityEngine;
using RoR2;
using EntityStates;
using BepInEx.Configuration;
using TemplarMod.Modules;
using RoR2.Projectile;
using TemplarMod.Templar.Content;
using TemplarMod.Templar.Components;

namespace TemplarMod.Templar.SkillStates
{
    public class MainState : GenericCharacterMain
    {
        private Animator animator;
        public LocalUser localUser;
        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = this.modelAnimator;
            this.FindLocalUser();
        }
        private void FindLocalUser()
        {
            if (this.localUser == null)
            {
                if (base.characterBody)
                {
                    foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
                    {
                        if (lu.cachedBody == base.characterBody)
                        {
                            this.localUser = lu;
                            break;
                        }
                    }
                }
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && characterBody.HasBuff(TemplarBuffs.AuraActiveBuff) && !base.gameObject.GetComponent<TemplarController>().auraActive)
            {
                FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
                fireProjectileInfo.projectilePrefab = base.gameObject.GetComponent<TemplarController>().templarAura;
                fireProjectileInfo.position = characterBody.corePosition;
                fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(characterBody.characterMotor.Motor.CharacterForward);
                fireProjectileInfo.owner = base.gameObject;
                fireProjectileInfo.damage = 1f;
                fireProjectileInfo.force = 0f;
                fireProjectileInfo.crit = false;
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                base.gameObject.GetComponent<TemplarController>().auraActive = true;
            }

            if (this.animator)
            {
                bool inCombat = false;
                if (!this.characterBody.outOfDanger || !this.characterBody.outOfCombat) inCombat = true;

                this.animator.SetBool("inCombat", inCombat);

                if (this.isGrounded) this.animator.SetFloat("airBlend", 0f);
                else this.animator.SetFloat("airBlend", 1f);
                
                if(!inCombat)
                {
                    if (this.animator.GetBool("isUnsheathed"))
                    {
                        this.animator.SetBool("isUnsheathed", false);
                        if(this.characterBody.isSprinting) PlayAnimationOnAnimator(this.animator, "Transition", "SprintToSafe");
                        else if (!this.characterBody.GetNotMoving()) PlayAnimationOnAnimator(this.animator, "Transition", "RunToSafe");
                        else PlayAnimationOnAnimator(this.animator, "Transition", "ToSafe");
                    }
                    this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Combat"), 0f);
                }
                else if(this.animator.GetBool("isUnsheathed"))
                {
                    this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Combat"), 1f);
                }
            }
        }

        public override void ProcessJump()
        {

            if (this.hasCharacterMotor)
            {
                bool hopooFeather = false;
                bool waxQuail = false;

                if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
                {
                    int waxQuailCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
                    float horizontalBonus = 1f;
                    float verticalBonus = 1f;

                    if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
                    {
                        hopooFeather = true;
                        horizontalBonus = 1.5f;
                        verticalBonus = 1.5f;
                    }
                    else if (waxQuailCount > 0 && base.characterBody.isSprinting)
                    {
                        float v = base.characterBody.acceleration * base.characterMotor.airControl;

                        if (base.characterBody.moveSpeed > 0f && v > 0f)
                        {
                            waxQuail = true;
                            float num2 = Mathf.Sqrt(10f * (float)waxQuailCount / v);
                            float num3 = base.characterBody.moveSpeed / v;
                            horizontalBonus = (num2 + num3) / num3;
                        }
                    }

                    GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);

                    if (this.hasModelAnimator)
                    {
                        int layerIndex = base.modelAnimator.GetLayerIndex("Body");
                        if (layerIndex >= 0)
                        {
                            if (this.characterBody.isSprinting)
                            {
                                this.modelAnimator.CrossFadeInFixedTime("SprintJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
                            }
                            else
                            {
                                if (hopooFeather)
                                {
                                    this.modelAnimator.CrossFadeInFixedTime("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
                                }
                                else
                                {
                                    this.modelAnimator.CrossFadeInFixedTime("Jump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
                                }
                            }
                        }
                    }

                    if (hopooFeather)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                        {
                            origin = base.characterBody.footPosition
                        }, true);
                    }
                    else if (base.characterMotor.jumpCount > 0)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            scale = base.characterBody.radius
                        }, true);
                    }

                    if (waxQuail)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
                        }, true);
                    }

                    base.characterMotor.jumpCount++;

                }
            }
        }
    }
}
