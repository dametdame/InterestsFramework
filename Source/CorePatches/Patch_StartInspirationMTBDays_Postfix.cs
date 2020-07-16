using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using DInterests;

namespace DInterests.CorePatches
{
    [HarmonyPatch(typeof(InspirationHandler))]
    [HarmonyPatch("StartInspirationMTBDays", MethodType.Getter)]
    class Patch_StartInspirationMTBDays_Postfix
    {
        private static void Postfix(ref float __result, InspirationHandler __instance)
        {
            InterestBase.HandleInspirationMTB(ref __result, __instance);
        }
    }
}
