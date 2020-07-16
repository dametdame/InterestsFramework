using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using HarmonyLib;

namespace DInterests
{
    [StaticConstructorOnStartup]
    public static class PatchCSALBase
    {

        public static Type mpu;

        static PatchCSALBase()
        {
            try
            {
                ((Action)(() =>
                {
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "children, school and learning"))
                    {
                        Log.Message("DInterests: Children, school and learning running, attempting to patch");

                        var harmony = new Harmony("io.github.dametri.interests");

                        mpu = AccessTools.TypeByName("Children.MorePawnUtilities");

                        var target1 = AccessTools.Method(mpu, "SetPawnSkillsAndPassions");
                        var invoke1 = AccessTools.Method(typeof(Patch_SetPawnSkillsAndPassions_PrefixPostfix), "Prefix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, prefix: new HarmonyMethod(invoke1));

                        var target2 = AccessTools.Method(mpu, "SetPawnSkillsAndPassions");
                        var invoke2 = AccessTools.Method(typeof(Patch_SetPawnSkillsAndPassions_PrefixPostfix), "Postfix");
                        if (target2 != null && invoke2 != null)
                            harmony.Patch(target2, postfix: new HarmonyMethod(invoke2));


                    }

                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
