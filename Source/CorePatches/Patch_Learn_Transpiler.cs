using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine.UI;

// Long function, choosing to use the Transpiler route since I think the hooks are solid

namespace DInterests
{
	[HarmonyPatch(typeof(SkillRecord))]
	[HarmonyPatch("Learn")]
	public class Patch_Learn_Transpiler
	{

		/* Code to delete:
		if (this.levelInt == 14)
		{  
		-----------------------------------------------------------------------------------------------------------------------------------------------
			if (this.passion == Passion.None)
			{
				TaleRecorder.RecordTale(TaleDefOf.GainedMasterSkillWithoutPassion, new object[]
				{
					this.pawn,
					this.def
				});
			}
			else
			{
				TaleRecorder.RecordTale(TaleDefOf.GainedMasterSkillWithPassion, new object[]
				{
					this.pawn,
					this.def
				});
			}
		}
		-----------------------------------------------------------------------------------------------------------------------------------------------
		if (this.levelInt >= 20)
		 */

		// Code to replace in IL (between dashes):

		// /* 0x001772AD 7B33280004   */ IL_00A9: ldfld     int32 RimWorld.SkillRecord::levelInt
		// /* 0x001772B2 1F0E         */ IL_00AE: ldc.i4.s  14
		// /* 0x001772B4 3350         */ IL_00B0: bne.un.s  IL_0102
		// -----------------------------------------------------------------------------------------------------------------------------------------------
		// /* 0x001772B6 02           */ IL_00B2: ldarg.0
		// /* 0x001772B7 7B34280004   */ IL_00B3: ldfld     valuetype RimWorld.Passion RimWorld.SkillRecord::passion
		// /* 0x001772BC 2D25         */ IL_00B8: brtrue.s  IL_00DF
		// /* 0x001772BE 7EA4380004   */ IL_00BA: ldsfld    class RimWorld.TaleDef RimWorld.TaleDefOf::GainedMasterSkillWithoutPassion
		// /* 0x001772C3 18           */ IL_00BF: ldc.i4.2
		// /* 0x001772C4 8D0A000001   */ IL_00C0: newarr    [mscorlib]System.Object
		// /* 0x001772C9 25           */ IL_00C5: dup
		// /* 0x001772CA 16           */ IL_00C6: ldc.i4.0
		// /* 0x001772CB 02           */ IL_00C7: ldarg.0
		// /* 0x001772CC 7B31280004   */ IL_00C8: ldfld     class Verse.Pawn RimWorld.SkillRecord::pawn
		// /* 0x001772D1 A2           */ IL_00CD: stelem.ref
		// /* 0x001772D2 25           */ IL_00CE: dup
		// /* 0x001772D3 17           */ IL_00CF: ldc.i4.1
		// /* 0x001772D4 02           */ IL_00D0: ldarg.0
		// /* 0x001772D5 7B32280004   */ IL_00D1: ldfld     class RimWorld.SkillDef RimWorld.SkillRecord::def
		// /* 0x001772DA A2           */ IL_00D6: stelem.ref
		// /* 0x001772DB 285D490006   */ IL_00D7: call      class RimWorld.Tale RimWorld.TaleRecorder::RecordTale(class RimWorld.TaleDef, object[])
		// /* 0x001772E0 26           */ IL_00DC: pop
		// /* 0x001772E1 2B23         */ IL_00DD: br.s      IL_0102
		// /* 0x001772E3 7EA3380004   */ IL_00DF: ldsfld    class RimWorld.TaleDef RimWorld.TaleDefOf::GainedMasterSkillWithPassion
		// /* 0x001772E8 18           */ IL_00E4: ldc.i4.2
		// /* 0x001772E9 8D0A000001   */ IL_00E5: newarr    [mscorlib]System.Object
		// /* 0x001772EE 25           */ IL_00EA: dup
		// /* 0x001772EF 16           */ IL_00EB: ldc.i4.0
		// /* 0x001772F0 02           */ IL_00EC: ldarg.0
		// /* 0x001772F1 7B31280004   */ IL_00ED: ldfld     class Verse.Pawn RimWorld.SkillRecord::pawn
		// /* 0x001772F6 A2           */ IL_00F2: stelem.ref
		// /* 0x001772F7 25           */ IL_00F3: dup
		// /* 0x001772F8 17           */ IL_00F4: ldc.i4.1
		// /* 0x001772F9 02           */ IL_00F5: ldarg.0
		// /* 0x001772FA 7B32280004   */ IL_00F6: ldfld     class RimWorld.SkillDef RimWorld.SkillRecord::def
		// /* 0x001772FF A2           */ IL_00FB: stelem.ref
		// /* 0x00177300 285D490006   */ IL_00FC: call      class RimWorld.Tale RimWorld.TaleRecorder::RecordTale(class RimWorld.TaleDef, object[])
		// /* 0x00177305 26           */ IL_0101: pop
		// -----------------------------------------------------------------------------------------------------------------------------------------------
		// /* 0x00177306 02           */ IL_0102: ldarg.0
		// /* 0x00177307 7B33280004   */ IL_0103: ldfld int32 RimWorld.SkillRecord::levelInt
		// /* 0x0017730C 1F14         */ IL_0108: ldc.i4.s  20
		// /* 0x0017730E 322C         */ IL_010A: blt.s IL_0138



		private const int startOffset = 2;
		private const int endOffset = -2;

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var found = false;
			int startIndex = -1, endIndex = -1;
			var codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
			{
				if (startIndex == -1 && codes[i].opcode == OpCodes.Ldc_I4_S)
				{
					var type = codes[i].operand;
					if (type.GetType() == typeof(SByte))
					{
						if ((SByte)type == (SByte)14)
						{
							startIndex = i + startOffset;
						}
					}
				}
				else if (startIndex > -1 && codes[i].opcode == OpCodes.Ldfld)
				{
					var type = codes[i].operand as System.Reflection.FieldInfo;
					if (type != null)
					{
						if (type.Name == "levelInt")
						{
							endIndex = i + endOffset;
							found = true;
							break;
						}
					}

				}
			}
			if (found)
			{
				var loadThis = new CodeInstruction(OpCodes.Ldarg_0);
				var loadThisForPawn = new CodeInstruction(OpCodes.Ldarg_0);
				var p = typeof(SkillRecord).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
				var pcall = new CodeInstruction(OpCodes.Ldfld, p);

				var mi = AccessTools.Method(typeof(InterestBase), nameof(InterestBase.RecordMasterTale));
				var modCall = new CodeInstruction(OpCodes.Call, mi);
				codes[startIndex] = loadThis;
				codes[startIndex + 1] = loadThisForPawn;
				codes[startIndex + 2] = pcall;
				codes[startIndex + 3] = modCall;
				for (var x = startIndex + 4; x <= endIndex; x++)
				{
					codes[x].opcode = OpCodes.Nop;
				}
			}
			return codes.AsEnumerable();
		}

	}
}
