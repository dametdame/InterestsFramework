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

// Transpiler needed since this is a GUI element and I want to avoid destructive postfixes

namespace DInterests
{
	[HarmonyPatch(typeof(SkillUI))]
	[HarmonyPatch("DrawSkill")]
	[HarmonyPatch(new Type[] {typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) })]
    public class Patch_DrawSkill_Transpiler
    {
		/* Code to replace:
		 
			if (!skill.TotallyDisabled)
			{
			-----------------------------------------------------------------------------------------------------------------------------------------------
				if (skill.passion > Passion.None)
				{
					Texture2D image = (skill.passion == Passion.Major) ? SkillUI.PassionMajorIcon : SkillUI.PassionMinorIcon;
					GUI.DrawTexture(position, image);
				}
			-----------------------------------------------------------------------------------------------------------------------------------------------
			Rect rect2 = new Rect(position.xMax, 0f, holdingRect.width - position.xMax, holdingRect.height);
		*/

		// IL code to delete (between dashes):

		// /* 0x001FAD0A 6FDE450006   */ IL_0076: callvirt instance bool RimWorld.SkillRecord::get_TotallyDisabled()
		// /* 0x001FAD0F 2D77         */ IL_007B: brtrue.s IL_00F4
		// -----------------------------------------------------------------------------------------------------------------------------------------------
		// /* 0x001FAD11 02           */ IL_007D: ldarg.0
		// /* 0x001FAD12 7B34280004   */ IL_007E: ldfld valuetype RimWorld.Passion RimWorld.SkillRecord::passion
		// /* 0x001FAD17 16           */ IL_0083: ldc.i4.0
		// /* 0x001FAD18 311F         */ IL_0084: ble.s IL_00A5
		// /* 0x001FAD1A 02           */ IL_0086: ldarg.0
		// /* 0x001FAD1B 7B34280004   */ IL_0087: ldfld valuetype RimWorld.Passion RimWorld.SkillRecord::passion
		// /* 0x001FAD20 18           */ IL_008C: ldc.i4.2
		// /* 0x001FAD21 2E07         */ IL_008D: beq.s IL_0096
		// /* 0x001FAD23 7E27320004   */ IL_008F: ldsfld    class [UnityEngine.CoreModule]UnityEngine.Texture2D RimWorld.SkillUI::PassionMinorIcon
		// /* 0x001FAD28 2B05         */ IL_0094: br.s IL_009B
		// /* 0x001FAD2A 7E28320004   */ IL_0096: ldsfld    class [UnityEngine.CoreModule]UnityEngine.Texture2D RimWorld.SkillUI::PassionMajorIcon
		// /* 0x001FAD2F 1305         */ IL_009B: stloc.s V_5
		// /* 0x001FAD31 07           */ IL_009D: ldloc.1
		// /* 0x001FAD32 1105         */ IL_009E: ldloc.s V_5
		// /* 0x001FAD34 28C609000A   */ IL_00A0: call void[UnityEngine.IMGUIModule] UnityEngine.GUI::DrawTexture(valuetype[UnityEngine.CoreModule] UnityEngine.Rect, class [UnityEngine.CoreModule] UnityEngine.Texture)
		// -----------------------------------------------------------------------------------------------------------------------------------------------
		// /* 0x001FAD39 1201         */ IL_00A5: ldloca.s V_1
		// /* 0x001FAD3B 28BF01000A   */ IL_00A7: call instance float32[UnityEngine.CoreModule] UnityEngine.Rect::get_xMax()

		private const int startOffset = 1;
		private const int endOffset = 2;

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var found = false;
			int startIndex = -1, endIndex = -1;
			var codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
			{
				if (startIndex == -1 && codes[i].opcode == OpCodes.Ldfld)
				{
					var type = codes[i].operand as System.Reflection.FieldInfo;
					if (type != null)
					{
						if (type.FieldType == typeof(Passion))
						{
							startIndex = i - startOffset;
						}
					}
				}
				else if (startIndex > -1 && codes[i].opcode == OpCodes.Call)
				{
					//Log.Message("call " + codes[i].operand);
					var type = codes[i].operand as System.Reflection.MethodInfo;
					if (type != null)
					{
						//Log.Message("name " + type.Name);
						if (type.Name == "get_xMax") 
						{
							endIndex = i - endOffset;
							found = true;
							break;
						}
					}

				}
			}
			if (found) {
				var loadPos = new CodeInstruction(OpCodes.Ldloc_S, 1);
				var loadSkill = new CodeInstruction(OpCodes.Ldarg_0);
				var mi = AccessTools.Method(typeof(InterestBase), nameof(InterestBase.DrawSkill));
				var modCall = new CodeInstruction(OpCodes.Call, mi);
				codes[startIndex] = loadPos;
				codes[startIndex+1] = loadSkill;
				codes[startIndex+2] = modCall;
				for (var x = startIndex+3; x <= endIndex; x++)
				{
					codes[x].opcode = OpCodes.Nop;
				}
			}
			return codes.AsEnumerable();
		}
		
	}
}
