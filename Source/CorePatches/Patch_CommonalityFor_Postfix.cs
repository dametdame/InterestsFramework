using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

// Replicates original function, but I think this is pretty light and safe

namespace DInterests
{
    [HarmonyPatch(typeof(InspirationWorker))]
    [HarmonyPatch("CommonalityFor")]
    class Patch_CommonalityFor_Postfix
    {
        private static void Postfix(ref float __result, Pawn pawn, InspirationDef ___def)
        {
			float num = 1f;
			if (pawn.skills != null && ___def.associatedSkills != null)
			{
				for (int i = 0; i < ___def.associatedSkills.Count; i++)
				{
					SkillDef skillDef = ___def.associatedSkills[i];
					for (int j = 0; j < pawn.skills.skills.Count; j++)
					{
						SkillRecord skillRecord = pawn.skills.skills[j];
						if (skillDef == skillRecord.def)
						{
							int s = (int)pawn.skills.skills[j].passion;
							num = Mathf.Max(num, InterestBase.GetInspirationFactor(s));
						}
					}
				}
			}
			__result = ___def.baseCommonality * num;
		}
    }
}
