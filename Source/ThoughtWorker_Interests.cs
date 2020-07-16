using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

// Overrides base class for the ThoughtDef "DoingPassionateWork"
// Patch that does this is in 1.1/Patches/PatchThoughts_Situation_Special

namespace DInterests
{
    class ThoughtWorker_Interests : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
			SkillDef active = InterestBase.GetActiveSkill(p);
			if (active == null)
				return ThoughtState.Inactive;

			SkillRecord skill = p.skills.GetSkill(active);
			if (skill == null)
			{
				return ThoughtState.Inactive;
			}
			int passion = (int)skill.passion;

			// these could return -1 but passion won't be -1
			var minor = InterestBase.interestList.FindIndex(x => x.defName == "DMinorPassion");
			if (passion == minor)
				return ThoughtState.ActiveAtStage(0);
			var major = InterestBase.interestList.FindIndex(x => x.defName == "DMajorPassion");
			if (passion == major)
				return ThoughtState.ActiveAtStage(1);
			return ThoughtState.Inactive;
		}
    }
}
