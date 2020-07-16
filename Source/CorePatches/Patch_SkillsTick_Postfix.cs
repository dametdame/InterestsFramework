using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;

namespace DInterests.CorePatches
{
    [HarmonyPatch(typeof(Pawn_SkillTracker))]
    [HarmonyPatch("SkillsTick")]
    class Patch_SkillsTick_Postfix
    {
        private static void Postfix(Pawn_SkillTracker __instance, Pawn ___pawn)
        { 
            InterestBase.HandleSkillTick(__instance, ___pawn);   
        }
    }
}
