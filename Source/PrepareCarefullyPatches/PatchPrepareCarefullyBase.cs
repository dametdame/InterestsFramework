using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using EdB.PrepareCarefully;
using Verse;
using HarmonyLib;

namespace DInterests
{
    [StaticConstructorOnStartup]
    public static class PatchPrepareCarefullyBase
    {

        public static Type ps;
        public static Type cp;

        static PatchPrepareCarefullyBase()
        {
            try
            {
                ((Action)(() =>
                {
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "EdB Prepare Carefully"))
                    {
                        Log.Message("DInterests: EdB Prepare Carefully running, attempting to patch");

                        var harmony = new Harmony("io.github.dametri.interests");

                        PatchPrepareCarefullyBase.ps = AccessTools.TypeByName("EdB.PrepareCarefully.PanelSkills");
                        PatchPrepareCarefullyBase.cp = AccessTools.TypeByName("EdB.PrepareCarefully.CustomPawn");


                        var target1 = AccessTools.Method(PatchPrepareCarefullyBase.ps, "DecreasePassion");
                        var invoke1 = AccessTools.Method(typeof(Patch_DecreasePassion_Prefix1), "Prefix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, prefix: new HarmonyMethod(invoke1));

                        var target2 = AccessTools.Method(PatchPrepareCarefullyBase.cp, "DecreasePassion");
                        var invoke2 = AccessTools.Method(typeof(Patch_DecreasePassion_Prefix2), "Prefix");
                        if (target2 != null && invoke2 != null)
                            harmony.Patch(target2, prefix: new HarmonyMethod(invoke2));

                        var target3 = AccessTools.Method(PatchPrepareCarefullyBase.ps, "IncreasePassion");
                        var invoke3 = AccessTools.Method(typeof(Patch_IncreasePassion_Prefix1), "Prefix");
                        if (target3 != null && invoke3 != null)
                            harmony.Patch(target3, prefix: new HarmonyMethod(invoke3));

                        var target4 = AccessTools.Method(PatchPrepareCarefullyBase.cp, "IncreasePassion");
                        var invoke4 = AccessTools.Method(typeof(Patch_IncreasePassion_Prefix2), "Prefix");
                        if (target4 != null && invoke4 != null)
                            harmony.Patch(target4, prefix: new HarmonyMethod(invoke4));

                        var target6 = AccessTools.Method(PatchPrepareCarefullyBase.ps, "DrawPanelContent");
                        var invoke6 = AccessTools.Method(typeof(Patch_DrawPanelContent_Transpiler), "Transpiler");
                        if (target6 != null && invoke6 != null)
                            harmony.Patch(target6, transpiler: new HarmonyMethod(invoke6));
                    }

                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }

        public static int DecreasePassion(int passion, Verse.Pawn pawn, SkillDef def)
        {
            int oldPassion = passion;
            do
            {
                passion--;
                if (passion < 0)
                    passion = InterestBase.interestList.Count - 1;
                if (InterestBase.interestList[passion].IsValidForPawn(pawn, def))
                    return passion;
            } while (passion != oldPassion);
                return oldPassion;
        }

        public static int IncreasePassion(int passion, Verse.Pawn pawn, SkillDef def)
        {
            int oldPassion = passion;
            do
            {
                passion++;
                if (passion > InterestBase.interestList.Count - 1)
                    passion = 0;
                if (InterestBase.interestList[passion].IsValidForPawn(pawn, def))
                    return passion;
            } while (passion != oldPassion);
            return oldPassion;
        }


        public static void DrawInterest(Passion passion, Rect position)
        {
            Texture2D image;
            if (InterestBase.interestList.GetDefaultIndex() == (int)passion)
                image = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/NoPassion", true);
            else
                image = InterestBase.interestList[(int)passion].GetTexture();
            GUI.color = Color.white;
            if (image != null) 
                GUI.DrawTexture(position, image);
        }
    }
}
