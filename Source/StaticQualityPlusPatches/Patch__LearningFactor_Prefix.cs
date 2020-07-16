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
using YouDoYou;
using static_quality_plus;

namespace DInterests
{

    class Patch__LearningFactor_Prefix
    {
        private static bool Prefix(float global_lf, Passion passion, bool learning_saturated, ref float __result)
        {
			bool fastLearning = DebugSettings.fastLearning;
			if (fastLearning)
			{
				__result = 200f;
			}
			else
			{
				float num = global_lf - 1f;
				InterestDef id = InterestBase.interestList[(int)passion];
				float toAdd = id.learnFactor;
				num += toAdd;
				if (learning_saturated)
				{
					num *= 0.2f;
				}
				__result = num;
			}
			return false;
		}
    }
}
