using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using EdB.PrepareCarefully;
using HarmonyLib;

// Decrease Passion for CustomPawn

namespace DInterests
{
    class Patch_DecreasePassion_Prefix2
    {
        public static bool Prefix(SkillDef def, object __instance, ref Verse.Pawn ___pawn)
        {

            var IsSkillDisabled = AccessTools.Method(PatchPrepareCarefullyBase.cp, "IsSkillDisabled");
            bool f = (bool)IsSkillDisabled.Invoke(__instance, new[] { def });
            if (f)
            //if (__instance.IsSkillDisabled(def))
                return false;
           
            var cur = AccessTools.Field(PatchPrepareCarefullyBase.cp, "currentPassions");
            Dictionary<SkillDef, Passion> currentPassions = cur.GetValue(__instance) as Dictionary<SkillDef, Passion>;
            int passion = (int)currentPassions[def];
            //int passion = (int)__instance.currentPassions[def];
            passion = PatchPrepareCarefullyBase.DecreasePassion(passion, ___pawn, def);
            ___pawn.skills.GetSkill(def).passion = (Passion)passion;
            
            var CopySkillsAndPassionsToPawn = AccessTools.Method(PatchPrepareCarefullyBase.cp, "CopySkillsAndPassionsToPawn");
            CopySkillsAndPassionsToPawn.Invoke(__instance, null);
           // __instance.CopySkillsAndPassionsToPawn();
            return false;
        }
    }
}
