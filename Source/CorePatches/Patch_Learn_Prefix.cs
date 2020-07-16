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
    [HarmonyPatch(typeof(SkillRecord))]
    [HarmonyPatch("Learn")]
    class Patch_Learn_Prefix
    {
        private static bool Prefix(ref float xp, ref bool direct, SkillRecord __instance, Pawn ___pawn)
        {
            InterestBase.HandleLearn(ref xp, ref direct, __instance, ___pawn);
            return true;
        }
    }
}
