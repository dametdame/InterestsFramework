using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using EdB.PrepareCarefully;
using HarmonyLib;

// Increase Passion for PanelSkills

namespace DInterests
{
    class Patch_IncreasePassion_Prefix1
    {

        public delegate void UpdateSkillPassionHandler(SkillDef skill, Passion level);

        public static bool Prefix(SkillRecord record, UpdateSkillPassionHandler ___SkillPassionUpdated)
        {
            Pawn pawn = AccessTools.Field(typeof(SkillRecord), "pawn").GetValue(record) as Pawn;
            if (pawn == null)
            {
                Log.Error("Failed to retrieve pawn in Patch_DecreasePassion_Prefix1, using original function");
                return true;
            }
            int passion = (int)record.passion;
            passion = PatchPrepareCarefullyBase.IncreasePassion(passion, pawn, record.def);
            ___SkillPassionUpdated(record.def, (Passion)passion);
            return false;
        }
    }
}
