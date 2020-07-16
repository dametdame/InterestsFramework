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

namespace DInterests.YouDoYouPatches
{

    class Patch_ConsiderPassion_Prefix
    {
        private static bool Prefix(Pawn pawn, WorkTypeDef workTypeDef, ref object __result, ref object __instance, ref float ___value)
        {
            int passion = getBestPassion(workTypeDef, pawn);
            //int passion = (int)pawn.skills.MaxPassionOfRelevantSkillsFor(workTypeDef);
            float mood = pawn.needs.mood.CurLevel;
            float value = InterestBase.interestList[passion].GetValue()/100f;
            Mathf.Clamp(value, -2.5f, 2.5f);
            var step = AccessTools.Method(PatchYouDoYouBase.priority, "Step");
            step.Invoke(__instance, new object[]{ mood*value });
            Mathf.Clamp(___value, 0.01f, 1f);
            __result = __instance;
            return false;
        }

        private static int getBestPassion(WorkTypeDef workDef, Pawn pawn)
        {
            int highestPassion = InterestBase.interestList.GetDefaultIndex();
            var skilltracker = pawn.skills;

            if (workDef.relevantSkills.Count == 0)
            {
                return highestPassion;
                
            }

            for (int i = 0; i < workDef.relevantSkills.Count; i++)
            {
                int passion2 = (int)skilltracker.GetSkill(workDef.relevantSkills[i]).passion;
                if (InterestBase.interestList[passion2].GetValue() > InterestBase.interestList[highestPassion].GetValue())
                    highestPassion = passion2;
            }
            return highestPassion;
        }

    }
}
