using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.HudOverlay;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using TemplarMod.Modules.BaseStates;
using TemplarMod.Templar.Content;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;

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
        public GameObject templarAura;
        public GameObject fireAura = TemplarAssets.auraWardCleansingFire;
        public GameObject convictAura = TemplarAssets.auraWardConviction;
        public GameObject prayAura = TemplarAssets.auraWardPrayer;
        public SkillDef skillDef;
        protected float stopwatch;
        public bool auraActive;
        public float auraRadiusRecalculate;
        public float auraPowerRecalculate;
        public float auraPowerBossTier;
        public float auraPowerUncommonTier;
        public float auraForceActivateRadius;
        public float barrageRadiusRecalculate;
        public bool auraForceActivateBonus;

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

        private void Start()
        {
            GenericSkill passiveSkillSelected = GetComponent<GenericSkill>();

            if (passiveSkillSelected != null && passiveSkillSelected.skillNameToken == ("KENKO_UNFORGIVEN_PASSIVE_AURA_FIRE"))
            {
                templarAura = fireAura;
                Log.Debug("Looking for fire aura?");
                Log.Debug("" + templarAura);
            }

            if (passiveSkillSelected != null && passiveSkillSelected.skillNameToken == ("KENKO_UNFORGIVEN_PASSIVE_AURA_CONVICT"))
            {
                templarAura = convictAura;
            }

            if (passiveSkillSelected != null && passiveSkillSelected.skillNameToken == ("KENKO_UNFORGIVEN_PASSIVE_AURA_PRAY"))
            {
                templarAura = prayAura;
            }

            // Log.Debug("" + passiveSkillSelected.skillNameToken);

        }
        private void FixedUpdate()
        {
            if (this.characterBody.HasBuff(TemplarBuffs.AuraForceActivateBuff))
            {
                auraForceActivateBonus = true;
            }
            else
            {
                auraForceActivateBonus = false;
            }

            if (auraForceActivateBonus == true)
            {
                auraForceActivateRadius = 2.0f;
            }
            else
            {
                auraForceActivateRadius = 1.0f;
            }

            auraPowerBossTier = (this.characterBody.inventory.GetItemCount(RoR2Content.Items.Pearl) + (this.characterBody.inventory.GetItemCount(RoR2Content.Items.ShinyPearl)));
            auraPowerUncommonTier = (this.characterBody.inventory.GetItemCount(RoR2Content.Items.LevelBonus) + (this.characterBody.inventory.GetItemCount(RoR2Content.Items.BonusGoldPackOnKill)));
            auraPowerRecalculate = (1f + (0.30f * auraPowerBossTier) + (0.15f * auraPowerUncommonTier));

            auraRadiusRecalculate = (((this.characterBody.armor * 0.2f) + 8f) * (auraPowerRecalculate + auraForceActivateRadius - 1f));
            barrageRadiusRecalculate = (((this.characterBody.armor * 0.2f) + 24f) * (auraPowerRecalculate + auraForceActivateRadius - 1f));

            TemplarAssets.holyBarragePrefab.transform.localScale = new Vector3(barrageRadiusRecalculate, 60f, barrageRadiusRecalculate);

            //templarAura.GetComponent<BuffWard>().radius = auraRadiusRecalculate;
            //templarAura.GetComponent<SphereCollider>().radius = auraRadiusRecalculate;

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

            if (this.characterBody.GetBuffCount(TemplarBuffs.AuraActiveBuff) >= 1)
            {
                characterBody.SetBuffCount(TemplarBuffs.AuraTimerBuff.buffIndex, 0);
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
                    if ((stopwatch >= 1f) | (this.characterBody.GetBuffCount(TemplarBuffs.AuraForceActivateBuff) == 1))
                    {
                        if ((this.characterBody.GetBuffCount(TemplarBuffs.AuraTimerBuff) == 1) | (this.characterBody.GetBuffCount(TemplarBuffs.AuraForceActivateBuff) == 1))
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