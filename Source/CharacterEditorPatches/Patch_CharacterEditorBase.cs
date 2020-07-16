using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using Verse;
using CharacterEditor;
using EdB.PrepareCarefully;

namespace DInterests.CharacterEditorPatches
{
    [StaticConstructorOnStartup]
    public static class Patch_CharacterEditorBase
    {

        static Patch_CharacterEditorBase()
        {
            try
            {
                ((Action)(() =>
                {
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Character Editor"))
                    {
                        Log.Message("DInterests: Character Editor running, attempting to patch");

                        var harmony = new Harmony("io.github.dametri.interests");

                        var target1 = AccessTools.Method(typeof(CharacterEditor.BlockBio), "ATogglePassion");
                        var invoke1 = AccessTools.Method(typeof(Patch_ATogglePassion_Prefix), "Prefix");
                        if (target1 != null && invoke1 != null)
                        {
                            harmony.Patch(target1, prefix: new HarmonyMethod(invoke1));
                        }
                        
                    }

                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }

        public static int IncreasePassion(int passion, Verse.Pawn pawn, SkillDef def)
        {
            return PatchPrepareCarefullyBase.IncreasePassion(passion, pawn, def);
        }
    }
}
