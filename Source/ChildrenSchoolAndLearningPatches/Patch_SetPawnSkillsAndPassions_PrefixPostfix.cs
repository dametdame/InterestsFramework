using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;

namespace DInterests
{
    class Patch_SetPawnSkillsAndPassions_PrefixPostfix
    {
       

        private static bool Prefix(Pawn pawn, bool OnlyIfSkillsAndPassionsAre0, out bool __state)
        {

			if (OnlyIfSkillsAndPassionsAre0)
			{
				List<SkillDef> allDefsListForReading;
				bool flag = false;
				bool flag2 = false;
				allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
				for (int i = 0; i < allDefsListForReading.Count; i++)
				{
					SkillDef skillDef = allDefsListForReading[i];
					SkillRecord skill = pawn.skills.GetSkill(skillDef);
					if (skill.Level != 0)
					{
						flag = true;
					}
					if (skill.passion != Passion.None)
					{
						flag2 = true;
					}
				}
				if (flag || flag2)
				{
					__state = true;
					return true;
				}
			}
			__state = false;
           return true;
        }

        private static void Postfix(Pawn pawn, Pawn mother1, Pawn father1, bool GenerateRandom, bool __state)
        {
			if (__state)
				return;

			InterestBase.Inherit(pawn, mother1, father1, GenerateRandom);
        }
    }
}
