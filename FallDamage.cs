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
        private static float HARDFALL_MIN = 1.1f;
        private static float HARDFALL_MAX = 8f;
        private static int   MAX_DAMAGE = 5;
        private static float HARDFALL_RANGE = HARDFALL_MAX - HARDFALL_MIN;

        private bool modenabled = true;
        private float falltimer = 0f;
        private int damage = 0;
        private int mode = 0;

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        new public string GetName() => "FallDamage";
        public override string GetVersion() => "0.2.0";
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
        private void OnHeroUpdate()
        {
            //Logs fall timer at the moment just before ground impact
            //if (HeroController.instance.fallTimer == 0f && falltimer > 0f) { Log(falltimer); }
            if (modenabled && HeroController.instance.fallTimer == 0f && this.falltimer > 0f)
            {
                Log("Fall detected!");
                switch (mode)
                {
                    //Regular mode
                    case 0:
                        if (this.falltimer > HARDFALL_MIN)
                        {
                            damage = (int)Math.Min(Math.Ceiling(((this.falltimer - HARDFALL_MIN) / HARDFALL_RANGE) * (float)MAX_DAMAGE), MAX_DAMAGE);
                            Log(damage);
                            //HurtHero(damage);
                        };
                        break;
                    //Glass Ankles mode
                    //TODO: Add code to change the values of HARDFALL_MIN, MAX, RANGE, and MAX_DAMAGE based on mode change
                    case 1:
                        if (this.falltimer > HARDFALL_MIN/2)
                        {
                            damage = (int)Math.Min(Math.Ceiling(((this.falltimer - HARDFALL_MIN) / HARDFALL_RANGE) * (float)MAX_DAMAGE), MAX_DAMAGE);
                            Log(damage);
                            //HurtHero(damage);
                        };
                        break;
                    default: throw new Exception("Tried to access a mode that does not exist!");
                }
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
    }
}

//TODO: Implement reaction for hardfalls upon scene change that don't meet HARDFALL_MIN (ie Crossroads well drop)
//TODO: Rewrite menu using Satchel Better Menus + add option for mode

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