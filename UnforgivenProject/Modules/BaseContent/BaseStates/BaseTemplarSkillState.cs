using EntityStates;
using RoR2;
using TemplarMod.Templar.Components;
using TemplarMod.Templar.Content;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace TemplarMod.Modules.BaseStates
{
    public abstract class BaseTemplarSkillState : BaseSkillState
    {
        protected TemplarController templarController;

        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        protected void RefreshState()
        {
            if (!templarController)
            {
                templarController = base.GetComponent<TemplarController>();
            }
        }
    }
}
