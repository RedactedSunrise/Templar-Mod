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

namespace TemplarMod.Templar.Components
{
    public class TemplarAuraFieldComponent : MonoBehaviour
    {
        private TemplarController templarController;
        private CharacterBody ownerBody;

        private void Awake()
        {
        }

        private void Start()
        {
            ownerBody = this.GetComponent<ProjectileController>().owner.GetComponent<CharacterBody>();
            templarController = ownerBody.gameObject.GetComponent<TemplarController>();
        }

        private void FixedUpdate()
        {
            base.transform.position = ownerBody.corePosition;

            base.GetComponent<BuffWard>().radius = templarController.auraRadiusRecalculate;

            base.GetComponent<SphereCollider>().radius = templarController.auraRadiusRecalculate;

            if (templarController && ownerBody.healthComponent.alive)
            {
                if (!ownerBody.HasBuff(TemplarBuffs.AuraActiveBuff))
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
            }
            else
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }

        }
    }
}

