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
    public abstract class BaseUnforgivenState : BaseState
    {
        protected TemplarController unforgivenController;

        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RefreshState();
        }
        protected void RefreshState()
        {
            if (!unforgivenController)
            {
                unforgivenController = base.GetComponent<TemplarController>();
            }
        }
    }
}
