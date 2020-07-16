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
	[HarmonyPatch(typeof(Pawn_SkillTracker))]
	[HarmonyPatch("MaxPassionOfRelevantSkillsFor")]
	class Patch_MaxPassionOfRelevantSkillsFor_Postfix
	{
		private static void Postfix(ref Passion __result, WorkTypeDef workDef, Pawn_SkillTracker __instance)
		{
			int highestPassion = InterestBase.interestList.GetDefaultIndex();

			if (workDef.relevantSkills.Count == 0)
			{
				__result = (Passion)highestPassion;
				return;
			}

			for (int i = 0; i < workDef.relevantSkills.Count; i++)
			{
				int passion2 = (int)__instance.GetSkill(workDef.relevantSkills[i]).passion;
				if (InterestBase.interestList[passion2] > InterestBase.interestList[highestPassion])
					highestPassion = passion2;
			}
			__result = (Passion)highestPassion;
		}
	}
}
