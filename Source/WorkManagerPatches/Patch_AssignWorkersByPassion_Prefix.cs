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
using WorkManager;

namespace DInterests
{

    class Patch_AssignWorkersByPassion_Prefix
    {
        private static bool Prefix(ref object __instance, ref HashSet<Pawn> ____capablePawns, ref HashSet<Pawn> ____managedPawns, ref HashSet<WorkTypeDef> ____managedWorkTypes, IEnumerable<WorkTypeDef> ____commonWorkTypes)
		{


			if (!____capablePawns.Any<Pawn>())
			{
				return false;
			}
			using (IEnumerator<Pawn> enumerator = ____capablePawns.Intersect(____managedPawns).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Pawn pawn = enumerator.Current;
					IEnumerable<WorkTypeDef> managedWorkTypes = ____managedWorkTypes;
					var isBadWork = AccessTools.Method(PatchWorkManagerBase.priority, "IsBadWork");
					var isPawnWorkTypeActive = AccessTools.Method(PatchWorkManagerBase.priority, "IsPawnWorkTypeActive");
					var setPawnWorkTypePriority = AccessTools.Method(PatchWorkManagerBase.priority, "SetPawnWorkTypePriority");
					foreach (WorkTypeDef workType in managedWorkTypes)
					{
						if (____commonWorkTypes.Contains(workType) || workType == WorkTypeDefOf.Doctor || workType == WorkTypeDefOf.Hunting || pawn.WorkTypeIsDisabled(workType)
						|| (bool)isBadWork.Invoke(__instance, new object [] { pawn, workType }) || (bool)isPawnWorkTypeActive.Invoke(__instance, new object [] {pawn, workType }))
							continue;
						Passion passion = pawn.skills.MaxPassionOfRelevantSkillsFor(workType);
						float val = InterestBase.interestList[(int)passion].GetValue();
						if (val > 0)
						{
							if (val >= 100)
							{
								int priority = 2;
								setPawnWorkTypePriority.Invoke(__instance, new object[] { pawn, workType, priority });
							}
							else
							{
								int priority = 3;
								setPawnWorkTypePriority.Invoke(__instance, new object[] { pawn, workType, priority });
							}
						}
						else if (val < -50)
						{
							int priority = 4;
							setPawnWorkTypePriority.Invoke(__instance, new object[] { pawn, workType, priority });
						}
					}
				}
			}
			return false;
        }


    }
}
