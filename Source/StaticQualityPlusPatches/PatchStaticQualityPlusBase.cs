using EdB.PrepareCarefully.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using YouDoYou;
using Verse;
using HarmonyLib;

namespace DInterests
{
    [StaticConstructorOnStartup]
    public static class PatchStaticQualityPlusBase
    {

        public static Type _SR;

        static PatchStaticQualityPlusBase()
        {
            try
            {
                ((Action)(() =>
                {
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.Contains("Static Quality Plus")))
                    {
                        Log.Message("DInterests: Static Quality Plus running, attempting to patch");

                        var harmony = new Harmony("io.github.dametri.interests");

                        _SR = AccessTools.TypeByName("static_quality_plus._SkillRecord");

                        var target1 = AccessTools.Method(_SR, "_LearningFactor");
                        var invoke1 = AccessTools.Method(typeof(Patch__LearningFactor_Prefix), "Prefix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, prefix: new HarmonyMethod(invoke1));

                        var target2 = AccessTools.Method(_SR, "_CheckPassionIncrease");
                        var invoke2 = AccessTools.Method(typeof(Patch_CheckPassionIncrease_Prefix), "Prefix");
                        if (target2 != null && invoke2 != null)
                            harmony.Patch(target2, prefix: new HarmonyMethod(invoke2));


                    }

                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
