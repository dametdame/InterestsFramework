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
using DInterests.YouDoYouPatches;

namespace DInterests
{
    [StaticConstructorOnStartup]
    public static class PatchYouDoYouBase
    {

        public static Type priority;

        static PatchYouDoYouBase()
        {
            try
            {
                ((Action)(() =>
                {
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "YouDoYou"))
                    {
                        Log.Message("DInterests: YouDoYou running, attempting to patch");

                        var harmony = new Harmony("io.github.dametri.interests");

                        priority = AccessTools.TypeByName("YouDoYou.Priority");

                        var target1 = AccessTools.Method(priority, "ConsiderPassion");
                        var invoke1 = AccessTools.Method(typeof(Patch_ConsiderPassion_Prefix), "Prefix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, prefix: new HarmonyMethod(invoke1));

                   
                    }

                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
