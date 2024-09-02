using BepInEx.Configuration;
using TemplarMod.Modules;
using TemplarMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RoR2.UI;
using R2API;
using R2API.Networking;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using TemplarMod.Templar.Components;
using TemplarMod.Templar.Content;
using TemplarMod.Templar.SkillStates;
using HG;
using EntityStates;
using R2API.Networking.Interfaces;
using EmotesAPI;
using System.Runtime.CompilerServices;
using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;
using System.Linq;

namespace TemplarMod.Templar
{
    public class TemplarSurvivor : SurvivorBase<TemplarSurvivor>
    {
        public override string assetBundleName => "unforgiven";
        public override string bodyName => "UnforgivenBody";
        public override string masterName => "UnforgivenMonsterMaster";
        public override string modelPrefabName => "mdlUnforgiven";
        public override string displayPrefabName => "UnforgivenDisplay";

        public const string TEMPLAR_PREFIX = TemplarPlugin.DEVELOPER_PREFIX + "_UNFORGIVEN_";
        public override string survivorTokenPrefix => TEMPLAR_PREFIX;

        internal static GameObject characterPrefab;

        public static SkillDef firstBreath;

        public static SkillDef templarHolyBolt;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = TEMPLAR_PREFIX + "NAME",
            subtitleNameToken = TEMPLAR_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texUnforgivenIcon"),
            bodyColor = TemplarAssets.templarColor,
            sortPosition = 8.9f,

            crosshair = Modules.CharacterAssets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 200f,
            healthRegen = 4.0f,
            armor = 24f, 
            damage = 12f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Model",
                },
                new CustomRendererInfo
                {
                    childName = "KatanaModel",
                },
                new CustomRendererInfo
                {
                    childName = "SheathModel",
                },
                new CustomRendererInfo
                {
                    childName = "EmpoweredSword"
                },
                new CustomRendererInfo
                {
                    childName = "ArmModel"
                }
        };

        public override UnlockableDef characterUnlockableDef => TemplarUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new TemplarItemDisplays();
        public override AssetBundle assetBundle { get; protected set; }
        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }
        public override void Initialize()
        {

            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Unforgiven");

            //if (!characterEnabled.Value)
            //    return;

            //need the character unlockable before you initialize the survivordef

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            TemplarConfig.Init();

            TemplarUnlockables.Init();

            base.InitializeCharacter();

            CameraParams.InitializeParams();

            DamageTypes.Init();

            TemplarStates.Init();
            TemplarTokens.Init();

            TemplarBuffs.Init(assetBundle);
            TemplarAssets.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            characterPrefab = bodyPrefab;

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<TemplarController>();
        }
        public void AddHitboxes()
        {
            Prefabs.SetupHitBoxGroup(characterModelObject, "MeleeHitbox", "MeleeHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "SteelTempestHitbox", "SteelTempestHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "SteelTempestSpinHitbox", "SteelTempestSpinHitbox");
        }

        public override void InitializeEntityStateMachines()
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(SkillStates.MainState), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your UnforgivenStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Dash");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Dash2");
        }

        #region skills
        public override void InitializeSkills()
        {
            bodyPrefab.AddComponent<TemplarPassive>();
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPassiveSkills();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtilitySkills();
            AddSpecialSkills();
            if (TemplarPlugin.scepterInstalled) InitializeScepter();
        }

        private void AddPassiveSkills()
        {
            TemplarPassive passive = bodyPrefab.GetComponent<TemplarPassive>();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;

            passive.unforgivenPassive = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = TEMPLAR_PREFIX + "PASSIVE_NAME",
                skillNameToken = TEMPLAR_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = TEMPLAR_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
                keywordTokens = new string[] { },
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, passive.unforgivenPassive);
        }

        private void AddPrimarySkills()
        {
            SteppedSkillDef unforgivenPrimary = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "TemplarZeal",
                    TEMPLAR_PREFIX + "PRIMARY_SWING_NAME",
                    TEMPLAR_PREFIX + "PRIMARY_SWING_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new SerializableEntityStateType(typeof(SlashCombo)),
                    "Weapon"
                ));
            unforgivenPrimary.stepCount = 2;
            unforgivenPrimary.stepGraceDuration = 0.1f;
            unforgivenPrimary.keywordTokens = new string[] { };

            Skills.AddPrimarySkills(bodyPrefab, unforgivenPrimary);

            templarHolyBolt = Skills.CreateSkillDef<SkillDef>(new SkillDefInfo
            {
                skillName = "TemplarHolyBolt",
                skillNameToken = TEMPLAR_PREFIX + "PRIMARY_SHOT_NAME",
                skillDescriptionToken = TEMPLAR_PREFIX + "PRIMARY_SHOT_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),

                activationState = new SerializableEntityStateType(typeof(ProjectileHolyPierce)),

                activationStateMachineName = "Weapon",
                interruptPriority = InterruptPriority.Skill,

                baseMaxStock = 3,
                baseRechargeInterval = 0f,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });
        }

        private void AddSecondarySkills()
        {
            SkillDef secondary = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Willbreaker",
                skillNameToken = TEMPLAR_PREFIX + "SECONDARY_STEEL_NAME",
                skillDescriptionToken = TEMPLAR_PREFIX + "SECONDARY_STEEL_DESCRIPTION",
                keywordTokens = new string[] { Tokens.agileKeyword, Tokens.unforgivenSwiftKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),

                activationState = new SerializableEntityStateType(typeof(ChainPull)),

                activationStateMachineName = "Weapon2",
                interruptPriority = InterruptPriority.Skill,

                baseMaxStock = 2,
                baseRechargeInterval = 8f,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = true,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, secondary);
        }

        private void AddUtilitySkills()
        {
            SkillDef HolyBarrage = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HolyRain",
                skillNameToken = TEMPLAR_PREFIX + "UTILITY_SWEEP_NAME",
                skillDescriptionToken = TEMPLAR_PREFIX + "UTILITY_SWEEP_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new SerializableEntityStateType(typeof(HolyBarrage)),
                activationStateMachineName = "Weapon",
                interruptPriority = InterruptPriority.Skill,

                baseRechargeInterval = 12f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = true,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

            });

            Skills.AddUtilitySkills(bodyPrefab, HolyBarrage);
        }

        private void AddSpecialSkills()
        {
            SkillDef LastBreath = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "LastBreath",
                skillNameToken = TEMPLAR_PREFIX + "SPECIAL_BREATH_NAME",
                skillDescriptionToken = TEMPLAR_PREFIX + "SPECIAL_BREATH_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SlashCombo)),
                activationStateMachineName = "Dash",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 9f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });

            Skills.AddSpecialSkills(bodyPrefab, LastBreath);
        }
        
        private void InitializeScepter()
        {
            firstBreath = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "FirstBreath",
                skillNameToken = TEMPLAR_PREFIX + "SPECIAL_SCEP_BREATH_NAME",
                skillDescriptionToken = TEMPLAR_PREFIX + "SPECIAL_SCEP_BREATH_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SlashCombo)),
                activationStateMachineName = "Dash",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 9f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(firstBreath, bodyName, SkillSlot.Special, 0);
        }
        
        #endregion skills

        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texDefaultSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
            //uncomment this when you have another skin
            defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshBody",
                "meshDefaultSword",
                "meshSheath",
                "meshEmpoweredSword",
                "meshArm");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            /*
            defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Tie"),
                    shouldActivate = true,
                }
            };
            */

            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin

            #region AscendencySkin

            ////creating a new skindef as we did before
            SkinDef ascendencySkin = Modules.Skins.CreateSkinDef(TEMPLAR_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texWhirlwindSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                TemplarUnlockables.ascendencySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            ascendencySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshAscendancyBody",
                "meshAscendancySword",//no gun mesh replacement. use same gun mesh
                "meshAscendancySheath",
                "meshAscendancySwordEmpowered",
                "meshAscendancyArm");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            ascendencySkin.rendererInfos[0].defaultMaterial = TemplarAssets.ascendencyMat;
            ascendencySkin.rendererInfos[1].defaultMaterial = TemplarAssets.ascendencyMat;
            ascendencySkin.rendererInfos[2].defaultMaterial = TemplarAssets.ascendencyMat;
            ascendencySkin.rendererInfos[3].defaultMaterial = TemplarAssets.ascendencyMat;
            ascendencySkin.rendererInfos[4].defaultMaterial = TemplarAssets.ascendencyMat;

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(ascendencySkin);

            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins


        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            TemplarAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(HealthComponent_TakeDamage);
            if (TemplarPlugin.emotesInstalled) Emotes();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Emotes()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                var skele = TemplarAssets.mainAssetBundle.LoadAsset<GameObject>("unforgiven_emoteskeleton");
                CustomEmotesAPI.ImportArmature(TemplarSurvivor.characterPrefab, skele);
            };
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && self.alive || !self.godMode || self.ospTimer <= 0f)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();

                if (attackerBody && attackerBody.HasBuff(TemplarBuffs.AflameBuff))
                {
                    damageInfo.damageType |= DamageType.IgniteOnHit;
                    attackerBody.ClearTimedBuffs(TemplarBuffs.AflameBuff);
                }
            }
            orig.Invoke(self, damageInfo);
        }
        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("UnforgivenBody"))
            {
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC") i.token = "Passive";
                }
            }
        }

        
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(TemplarBuffs.TemplarGroundedBuff))
                {
                    self.maxJumpCount = 0;
                }
            }
        }
    }
}