using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using Modding;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace FallDamage
{
    public class FallDamage : Mod
    {
        new public string GetName() => "FallDamage";
        public override string GetVersion() => "0.1.0";
        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }
        public void OnHeroUpdate()
        {
            if (HeroController.instance.hero_state == ActorStates.hard_landing)
            {
                HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.other, 1, 1);
            }
        }
    }
}