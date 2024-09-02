using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.HudOverlay;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using TemplarMod.Templar.Content;
using UnityEngine;
using UnityEngine.Networking;

namespace TemplarMod.Templar.Components
{
    public class TemplarController : NetworkBehaviour
    {
        private CharacterBody characterBody;
        private ModelSkinController skinController;
        private ChildLocator childLocator;
        private CharacterModel characterModel;
        private Animator animator;
        private SkillLocator skillLocator;
        public GameObject templarAura = TemplarAssets.auraWardCleansingFire;
        protected float stopwatch;
        public bool auraActive;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            this.childLocator = modelLocator.modelTransform.GetComponentInChildren<ChildLocator>();
            this.animator = modelLocator.modelTransform.GetComponentInChildren<Animator>();
            this.characterModel = modelLocator.modelTransform.GetComponentInChildren<CharacterModel>();
            this.skillLocator = this.GetComponent<SkillLocator>();
            this.skinController = modelLocator.modelTransform.gameObject.GetComponent<ModelSkinController>();
        }
        private void FixedUpdate()
        {
            if (!this.characterBody.characterMotor.isGrounded)
            {
                if (skillLocator.primary.skillDef.skillIndex == SkillCatalog.FindSkillIndexByName("TemplarZeal"))
                {
                    skillLocator.primary.SetSkillOverride(base.gameObject, TemplarSurvivor.templarHolyBolt, GenericSkill.SkillOverridePriority.Contextual);
                    skillLocator.primary.SetBonusStockFromBody(characterBody.maxJumpCount - 1);
                    skillLocator.primary.stock = skillLocator.primary.maxStock;
                }
            }
            else
            {
                if (skillLocator.primary.skillDef.skillIndex == SkillCatalog.FindSkillIndexByName(TemplarSurvivor.templarHolyBolt.skillName))
                {
                    skillLocator.primary.UnsetSkillOverride(base.gameObject, TemplarSurvivor.templarHolyBolt, GenericSkill.SkillOverridePriority.Contextual);
                    skillLocator.primary.SetBonusStockFromBody(0);
                    skillLocator.primary.stock = skillLocator.primary.maxStock;
                }
            }

            if (!this.characterBody.outOfCombat && !this.characterBody.HasBuff(TemplarBuffs.AuraActiveBuff))
            {
                if (!this.characterBody.HasBuff(TemplarBuffs.AuraTimerBuff))
                {
                    for (int i = 5; i > 0; i--)
                    {
                        if (NetworkServer.active)
                        {
                            characterBody.AddBuff(TemplarBuffs.AuraTimerBuff);
                        }
                    }
                }
                else
                {
                    stopwatch += Time.deltaTime;
                    if (stopwatch >= 1f)
                    {
                            if (this.characterBody.GetBuffCount(TemplarBuffs.AuraTimerBuff) == 1)
                            {
                                if (NetworkServer.active)
                                {
                                characterBody.AddBuff(TemplarBuffs.AuraActiveBuff);
                                }
                        }
                        if (NetworkServer.active)
                        {
                            characterBody.RemoveBuff(TemplarBuffs.AuraTimerBuff);
                        }
                            stopwatch = 0f;
                    }
                }
            }
            else if (this.characterBody.outOfCombat)
            {
                if (NetworkServer.active)
                {
                    if (this.characterBody.HasBuff(TemplarBuffs.AuraActiveBuff))
                    {
                        characterBody.SetBuffCount(TemplarBuffs.AuraActiveBuff.buffIndex, 0);
                        auraActive = false;
                    }

                    if (this.characterBody.HasBuff(TemplarBuffs.AuraTimerBuff))
                    {
                        characterBody.SetBuffCount(TemplarBuffs.AuraTimerBuff.buffIndex, 0);
                    }
                }
            }
        }
    }
}