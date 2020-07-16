using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimHUD;
using Verse;
using UnityEngine;

namespace DInterests
{

    class Patch_CheckPassionIncrease_Prefix
    {
        private static bool Prefix(Pawn pawn, SkillDef skilldef)
        {
			System.Random random = new System.Random();
			int num = random.Next(1, 11);
			SkillRecord sr = pawn.skills.GetSkill(skilldef);
			bool flag = sr.Level > num;
			if (flag)
			{
				int betterPassion = InterestBase.GetBetterPassion((int)sr.passion);
				sr.passion = (Passion)betterPassion;

				int num2 = 0;
				List<SkillRecord> list = new List<SkillRecord>();
				foreach (SkillRecord skillRecord in pawn.skills.skills)
				{
					bool flag4 = skillRecord.passion > Passion.None && skillRecord.def != skilldef;
					if (flag4)
					{
						num2++;
						list.Add(skillRecord);
					}
				}
				bool flag5 = sqp_mod.settings.passion_cap && num2 > 4;
				if (flag5)
				{
					num = random.Next(0, num2);
					SkillRecord skillRecord2 = list[num];
					skillRecord2.passion = (Passion)InterestBase.GetWorsePassion((int)skillRecord2.passion);
				}
			}
			return false;
		}
    }
}
