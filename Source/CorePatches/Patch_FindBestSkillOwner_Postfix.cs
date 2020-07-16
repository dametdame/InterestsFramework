using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;


// Replicated initial code, but light and unlikely to be changed (and transpiling would be a pain for this)

namespace DInterests
{
    [HarmonyPatch(typeof(Page_ConfigureStartingPawns))]
    [HarmonyPatch("FindBestSkillOwner")]
    class Patch_FindBestSkillOwner_Postfix
    {
        private static void Postfix(ref Pawn __result, SkillDef skill)
        {
			Pawn pawn = Find.GameInitData.startingAndOptionalPawns[0];
			SkillRecord skillRecord = pawn.skills.GetSkill(skill);
			for (int i = 1; i < Find.GameInitData.startingPawnCount; i++)
			{
				SkillRecord skill2 = Find.GameInitData.startingAndOptionalPawns[i].skills.GetSkill(skill);
				var passionValue1 = InterestBase.GetValue((int)skillRecord.passion);
				var passionValue2 = InterestBase.GetValue((int)skill2.passion);
				if (skillRecord.TotallyDisabled || skill2.Level > skillRecord.Level || (skill2.Level == skillRecord.Level && passionValue2 > passionValue1))
				{
					pawn = Find.GameInitData.startingAndOptionalPawns[i];
					skillRecord = skill2;
				}
			}
			__result = pawn;
		}
    }
}
