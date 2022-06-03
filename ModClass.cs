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
    public class FallDamage : Mod, IMenuMod
    {
        private bool modenabled = true;

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        new public string GetName() => "FallDamage";
        public override string GetVersion() => "0.1.0";
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new IMenuMod.MenuEntry
                {
                    Name = "Fall Damage Active",
                    Description = null,
                    Values = new string[]
                    {
                        "ON",
                        "OFF"
                    },
                    Saver = opt => this.modenabled = opt switch
                    {
                        0 => true,
                        1 => false,
                        _ => throw new InvalidOperationException()
                    },
                    Loader = () => this.modenabled switch
                    {
                        true => 0,
                        false => 1
                    }
                },
            };
        }
        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }
        public void OnHeroUpdate()
        {
            if (HeroController.instance.hero_state == ActorStates.hard_landing && modenabled)
            {
                HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.other, 1, 1);
            }
        }
    }
}