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

namespace DInterests.RimHUDPatches
{

    class Patch_GetSkillPassionColor_Prefix   
    {
        private static bool Prefix(Passion passion, ref Color __result)
        {
            int p = (int)passion;
            InterestDef interest = InterestBase.interestList[p];
            float val = interest.GetValue();
            if (val < 0)
            {
                if (val / 75.0f < 1.0f)
                    __result = veryNegativeColor;
                else
                    __result = negativeColor;
            }
            else if (val == 0)
                __result = neutralColor;
            else
            {
                if (val / 75.0f > 1.0f)
                    __result = veryPositiveColor;
                else
                    __result = positiveColor;
            }
            return false;
        }

        static Color veryNegativeColor = new Color(0.27f, 0.50f, 0.70f);
        static Color negativeColor = new Color(0.0f, 0.75f, 1.0f);
        static Color neutralColor = new Color(1f, 1f, 1f);
        static Color positiveColor = new Color(1f, 0.9f, 0.7f);
        static Color veryPositiveColor = new Color(1f, 0.8f, 0.4f);
    }
}
