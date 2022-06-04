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
        private Menu MenuRef;

        private float falltimer = 0f;
        private int damage = 0;

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
                            GS.HARDFALL_MAX = 4f;
                        }
                        else //REGULAR MODE
                        {
                            GS.HARDFALL_MIN = 1.1f;
                            GS.HARDFALL_MAX = 6f;
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
                && HeroController.instance.hero_state != ActorStates.airborne
                && this.falltimer >= GS.HARDFALL_MIN
                && !(HeroController.instance.cState.spellQuake))
            {
                damage = (int)Math.Min(Math.Ceiling(((this.falltimer - GS.HARDFALL_MIN) / (GS.HARDFALL_MAX - GS.HARDFALL_MIN)) * (float)PlayerData.instance.maxHealthBase), PlayerData.instance.maxHealthBase);
                Log(damage);
                //HurtHero(damage);
            }
            this.falltimer = HeroController.instance.fallTimer;

            /*if (HeroController.instance.hero_state == ActorStates.hard_landing && modenabled)
            {
                HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.other, 1, 1);
            }*/
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

//TODO: Implement reaction for hardfalls upon scene change that don't meet HARDFALL_MIN (ie Crossroads well drop)
//BUG: Falling too fast through scene transition triggers damage

/*Falltimer max amounts:
 * Resting Grounds drop: 4.036
 * King's Pass drop: 4.086
 * Crossroads drop: 4.270
 * Cliffs leftside drop: 6.822
 * CoT elevator drop: 8.289
 * Abyss full drop: 11.000
 * 
 * Built in BIG_FALL_TIME = 1.1
 * 
 * 1 DAMAGE:
 * Regular: BIG_FALL_TIME (HARDFALL_MIN)
 * Glass Ankles: HARDFALL_MIN/2
 * 
 * KILL LIMIT
 * Regular: 8.0 (HARDFALL_MAX)
 * Glass Ankles: HARDFALL_MAX/2
 */