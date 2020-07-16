using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimHUD;
using RimWorld;
using Verse;

namespace DInterests.RimHUDPatches
{
    [StaticConstructorOnStartup]
    public static class Patch_RimHUDBase
    {

		public static Type hudPawnModel;
		public static Type hudSkillModel;

        static Patch_RimHUDBase()
        {
			try
			{
				((Action)(() =>
				{
					if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "RimHUD"))
					{
						Log.Message("DInterests: RimHUD running, attempting to patch");
						var harmony = new Harmony("io.github.dametri.interests");

						hudPawnModel = AccessTools.TypeByName("RimHUD.Data.Models.PawnModel");
						hudSkillModel = AccessTools.TypeByName("RimHUD.Data.Models.SkillModel");

						var hudTarget1 = AccessTools.Constructor(hudSkillModel, new[] { hudPawnModel, typeof(SkillDef) });
						var hudInvoke1 = AccessTools.Method(typeof(RimHUDPatches.Patch_SkillModelConstructor_Postfix), "Postfix");
						if (hudTarget1 != null && hudInvoke1 != null)
							harmony.Patch(hudTarget1, postfix: new HarmonyMethod(hudInvoke1));

						var hudTarget2 = AccessTools.Method(hudSkillModel, "GetSkillPassionColor");
						var hudInvoke2 = AccessTools.Method(typeof(RimHUDPatches.Patch_GetSkillPassionColor_Prefix), "Prefix");
						if (hudTarget2 != null && hudInvoke2 != null)
							harmony.Patch(hudTarget2, prefix: new HarmonyMethod(hudInvoke2));
					}

				}))();
			} 
			catch (TypeLoadException ex) { Log.Message(ex.ToString()); }

		}

    }
}
