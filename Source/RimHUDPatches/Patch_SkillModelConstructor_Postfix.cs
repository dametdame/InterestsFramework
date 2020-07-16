using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimHUD;
using Verse;
using DInterests;

namespace DInterests.RimHUDPatches
{

    class Patch_SkillModelConstructor_Postfix
    {

        private static void Postfix(Object model, SkillDef def, object __instance)
        {
           
            var r = AccessTools.PropertyGetter(Patch_RimHUDBase.hudPawnModel, "Base");
            Pawn b = (Pawn)r.Invoke(model, null);
            Pawn_SkillTracker st = b.skills;

            SkillRecord skillRecord = (st != null) ? st.GetSkill(def) : null;
            if (skillRecord == null)
                return;

            var label = AccessTools.Field(Patch_RimHUDBase.hudSkillModel, "<Label>k__BackingField");
            if (label == null)
                return;

            InterestDef i = InterestBase.interestList[(int)skillRecord.passion];
            string end = "";
            float val = i.GetValue();
            if (val < 0)
                end = new string('-', -(int)Math.Floor(val / 75.0f));
            else if (val > 0)
                end = new string('+', (int)Math.Floor(val / 75.0f));
            string l = def.LabelCap + end;
            label.SetValue(__instance, l);
        }
    }
}
