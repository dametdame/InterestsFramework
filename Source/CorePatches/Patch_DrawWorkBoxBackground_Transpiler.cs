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
	[HarmonyPatch(typeof(WidgetsWork))]
	[HarmonyPatch("DrawWorkBoxBackground")]
	public class Patch_DrawWorkBoxBackground_Transpiler
	{

		/* Code to replace
			Passion passion = p.skills.MaxPassionOfRelevantSkillsFor(workDef);
			-----------------------------------------------------------------------------------------------------------------------------------------------
			if (passion > Passion.None)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.4f);
				Rect position = rect;
				position.xMin = rect.center.x;
				position.yMin = rect.center.y;
				if (passion == Passion.Minor)
				{
					GUI.DrawTexture(position, WidgetsWork.PassionWorkboxMinorIcon);
				}
				else if (passion == Passion.Major)
				{
					GUI.DrawTexture(position, WidgetsWork.PassionWorkboxMajorIcon);
				}
			}
			-----------------------------------------------------------------------------------------------------------------------------------------------
			GUI.color = Color.white;

		*/


		// IL code to delete (between dashes):

		// /* 0x00200E21 6FD7450006   */ IL_00E9: callvirt  instance valuetype RimWorld.Passion RimWorld.Pawn_SkillTracker::MaxPassionOfRelevantSkillsFor(class Verse.WorkTypeDef)
		// /* 0x00200E26 1304         */ IL_00EE: stloc.s   V_4
		// -----------------------------------------------------------------------------------------------------------------------------------------------
		// /* 0x00200E28 1104         */ IL_00F0: ldloc.s   V_4
		// /* 0x00200E2A 16           */ IL_00F2: ldc.i4.0
		// /* 0x00200E2B 316B         */ IL_00F3: ble.s     IL_0160
		// /* 0x00200E2D 220000803F   */ IL_00F5: ldc.r4    1
		// /* 0x00200E32 220000803F   */ IL_00FA: ldc.r4    1
		// /* 0x00200E37 220000803F   */ IL_00FF: ldc.r4    1
		// /* 0x00200E3C 22CDCCCC3E   */ IL_0104: ldc.r4    0.4
		// /* 0x00200E41 737F02000A   */ IL_0109: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Color::.ctor(float32, float32, float32, float32)
		// /* 0x00200E46 28D803000A   */ IL_010E: call      void [UnityEngine.IMGUIModule]UnityEngine.GUI::set_color(valuetype [UnityEngine.CoreModule]UnityEngine.Color)
		// /* 0x00200E4B 02           */ IL_0113: ldarg.0
		// /* 0x00200E4C 1305         */ IL_0114: stloc.s   V_5
		// /* 0x00200E4E 1205         */ IL_0116: ldloca.s  V_5
		// /* 0x00200E50 0F00         */ IL_0118: ldarga.s  rect
		// /* 0x00200E52 28B303000A   */ IL_011A: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector2 [UnityEngine.CoreModule]UnityEngine.Rect::get_center()
		// /* 0x00200E57 7B9B01000A   */ IL_011F: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector2::x
		// /* 0x00200E5C 28E101000A   */ IL_0124: call      instance void [UnityEngine.CoreModule]UnityEngine.Rect::set_xMin(float32)
		// /* 0x00200E61 1205         */ IL_0129: ldloca.s  V_5
		// /* 0x00200E63 0F00         */ IL_012B: ldarga.s  rect
		// /* 0x00200E65 28B303000A   */ IL_012D: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector2 [UnityEngine.CoreModule]UnityEngine.Rect::get_center()
		// /* 0x00200E6A 7B9C01000A   */ IL_0132: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector2::y
		// /* 0x00200E6F 28E401000A   */ IL_0137: call      instance void [UnityEngine.CoreModule]UnityEngine.Rect::set_yMin(float32)
		// /* 0x00200E74 1104         */ IL_013C: ldloc.s   V_4
		// /* 0x00200E76 17           */ IL_013E: ldc.i4.1
		// /* 0x00200E77 330E         */ IL_013F: bne.un.s  IL_014F
		// /* 0x00200E79 1105         */ IL_0141: ldloc.s   V_5
		// /* 0x00200E7B 7ECC320004   */ IL_0143: ldsfld    class [UnityEngine.CoreModule]UnityEngine.Texture2D RimWorld.WidgetsWork::PassionWorkboxMinorIcon
		// /* 0x00200E80 28C609000A   */ IL_0148: call      void [UnityEngine.IMGUIModule]UnityEngine.GUI::DrawTexture(valuetype [UnityEngine.CoreModule]UnityEngine.Rect, class [UnityEngine.CoreModule]UnityEngine.Texture)
		// /* 0x00200E85 2B11         */ IL_014D: br.s      IL_0160
		// /* 0x00200E87 1104         */ IL_014F: ldloc.s   V_4
		// /* 0x00200E89 18           */ IL_0151: ldc.i4.2
		// /* 0x00200E8A 330C         */ IL_0152: bne.un.s  IL_0160
		// /* 0x00200E8C 1105         */ IL_0154: ldloc.s   V_5
		// /* 0x00200E8E 7ECD320004   */ IL_0156: ldsfld    class [UnityEngine.CoreModule]UnityEngine.Texture2D RimWorld.WidgetsWork::PassionWorkboxMajorIcon
		// /* 0x00200E93 28C609000A   */ IL_015B: call      void [UnityEngine.IMGUIModule]UnityEngine.GUI::DrawTexture(valuetype [UnityEngine.CoreModule]UnityEngine.Rect, class [UnityEngine.CoreModule]UnityEngine.Texture)
		// -----------------------------------------------------------------------------------------------------------------------------------------------
		// /* 0x00200E98 288102000A   */ IL_0160: call      valuetype [UnityEngine.CoreModule]UnityEngine.Color [UnityEngine.CoreModule]UnityEngine.Color::get_white()
		// /* 0x00200E9D 28D803000A   */ IL_0165: call      void [UnityEngine.IMGUIModule]UnityEngine.GUI::set_color(valuetype [UnityEngine.CoreModule]UnityEngine.Color)


		private const int startOffset = 2;
		private const int endOffset = -1;

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var found = false;
			int startIndex = -1, endIndex = -1;
			var codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
			{
				if (startIndex == -1 && codes[i].opcode == OpCodes.Callvirt)
				{
					var type = codes[i].operand as System.Reflection.MethodInfo;
					if (type != null)
					{
						if (type.Name == "MaxPassionOfRelevantSkillsFor")
						{
							startIndex = i + startOffset;
						}
					}
				}
				else if (startIndex > -1 && codes[i].opcode == OpCodes.Call)
				{
					var type = codes[i].operand as System.Reflection.MethodInfo;
					if (type != null)
					{
						if (type.Name == "get_white")
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
				var loadPos = new CodeInstruction(OpCodes.Ldloc_S, 4);
				var loadSkill = new CodeInstruction(OpCodes.Ldarg_0);
				var mi = AccessTools.Method(typeof(InterestBase), nameof(InterestBase.DrawWorkBoxBackground));
				var modCall = new CodeInstruction(OpCodes.Call, mi);
				codes[startIndex] = loadPos;
				codes[startIndex + 1] = loadSkill;
				codes[startIndex + 2] = modCall;
				for (var x = startIndex + 3; x <= endIndex; x++)
				{
					codes[x].opcode = OpCodes.Nop;
				}
			}
			return codes.AsEnumerable();
		}
		
	}
}
