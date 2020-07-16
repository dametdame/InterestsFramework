using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using CharacterEditor;
using DInterests.CharacterEditorPatches;

namespace DInterests
{

	class Patch_ATogglePassion_Prefix
	{
		private static bool Prefix(SkillRecord record)
		{ 
			Type EditorType = AccessTools.TypeByName("CharacterEditor.Editor");
			FieldInfo APIField = AccessTools.Field(typeof(PRMod), "API");
			object pr = AccessTools.CreateInstance(typeof(PRMod));	// I have absolutely no idea why we have to create an instance here, but
																	// I was unable to figure out static ref access in AccessTools, and this works
			object API = APIField.GetValue(pr);
			var p = AccessTools.Field(EditorType, "pawn");
			Pawn pawn = p.GetValue(API) as Pawn;
			record.passion = (Passion)Patch_CharacterEditorBase.IncreasePassion((int)record.passion, pawn, record.def);

			return false;
		}
	}
}
