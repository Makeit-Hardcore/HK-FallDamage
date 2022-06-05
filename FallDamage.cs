using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using Modding;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.BetterMenus;

namespace FallDamage
{
    public class FallDamage : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod, ITogglableMod
    {
        private float falltimer = 0f;
        private Menu MenuRef;

        public bool ToggleButtonInsideMenu => true;
        
        public static GlobalSettings GS { get; set; } = new GlobalSettings();

        new public string GetName() => "FallDamage";

        public override string GetVersion() => "0.2.0";
        
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modToggleDelegates)
        {
            if (MenuRef == null)
            {
                MenuRef = new Menu("FallDamage", new Element[]
{
                Blueprints.CreateToggle(
                    modToggleDelegates.Value,
                    "Mod Enabled",
                    ""
                ),
                new HorizontalOption(
                    "Mode Select",
                    "",
                    new string[]{"Regular","Glass Ankles"},
                    (setting) => {
                        GS.mode = setting;
                        if (setting == 1) //GLASS ANKLES MODE
                        {
                            GS.HARDFALL_MIN = 0.55f;
                            GS.HARDFALL_MAX = 4.5f;
                        }
                        else //REGULAR MODE
                        {
                            GS.HARDFALL_MIN = 1.1f;
                            GS.HARDFALL_MAX = 7f;
                        }
                        },
                    () => GS.mode
                )
});
            }
            return MenuRef.GetMenuScreen(modListMenu);
        }

        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }

        private void OnHeroUpdate()
        {
            if (HeroController.instance.fallTimer == 0f
                && this.falltimer > 0
                && HeroController.instance.hero_state != ActorStates.airborne
                && !HeroController.instance.cState.transitioning
                && !HeroController.instance.cState.spellQuake)
                {
                if (this.falltimer >= GS.HARDFALL_MIN)
                    { HurtHero((int)Math.Min(Math.Ceiling(((this.falltimer - GS.HARDFALL_MIN) / (GS.HARDFALL_MAX - GS.HARDFALL_MIN)) * (float)PlayerData.instance.maxHealthBase), PlayerData.instance.maxHealthBase));
                } else if (HeroController.instance.hero_state == ActorStates.hard_landing) //Forced hard landings, ie Dirtmouth well drop
                    { HurtHero(1);
                };
            };
            
            this.falltimer = HeroController.instance.fallTimer;
        }

        private void HurtHero(int dmg)
        {
            HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.other, dmg, 1);
        }

        public void Unload()
        {
            ModHooks.HeroUpdateHook -= OnHeroUpdate;
            Log("Mod unloaded");
        }

        public void OnLoadGlobal(GlobalSettings s) => GS = s;

        public GlobalSettings OnSaveGlobal() => GS;
    }

    public class GlobalSettings
    {
        public int mode = 0;
        public float HARDFALL_MIN = 1.1f;
        public float HARDFALL_MAX = 6f;
    }
}

/*Falltimer max amounts:
 * Resting Grounds drop: 4.036
 * King's Pass drop: 4.086
 * Crossroads drop: 4.270
 * Cliffs leftside drop: 6.822
 * CoT elevator drop: 8.289
 * Abyss full drop: 11.000
 * 
 * Built in BIG_FALL_TIME = 1.1
 */