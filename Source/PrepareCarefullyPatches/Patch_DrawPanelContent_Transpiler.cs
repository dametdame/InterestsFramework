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
using EdB.PrepareCarefully;


// Transpiler needed since this is a huge GUI method

namespace DInterests
{

	public class Patch_DrawPanelContent_Transpiler
	{

		/* Code to overwrite:
		 
			Passion passion = currentPawn.currentPassions[skillRecord.def];
			Texture2D image;
			-----------------------------------------------------------------------------------------------------------------------------------------------
			if (passion == Passion.Minor)
			{
				image = Textures.TexturePassionMinor;
			}
			else if (passion == Passion.Major)
			{
				image = Textures.TexturePassionMajor;
			}
			else
			{
				image = Textures.TexturePassionNone;
			}
			GUI.color = Color.white;
			GUI.DrawTexture(rect, image);
			-----------------------------------------------------------------------------------------------------------------------------------------------
		*/


		// // IL code to delete (between dashes):

		// /* 0x00028F29 28D101000A   */ IL_0189: call      instance void [UnityEngine]UnityEngine.Rect::set_y(float32)
		// /* 0x00028F2E 25           */ IL_018E: dup
		// /* 0x00028F2F 2D7E         */ IL_018F: brtrue.s  IL_020F
		// /* 0x00028F31 06           */ IL_0191: ldloc.0
		// /* 0x00028F32 7B37030004   */ IL_0192: ldfld     class [mscorlib]System.Collections.Generic.Dictionary`2<class ['Assembly-CSharp']RimWorld.SkillDef, valuetype ['Assembly-CSharp']RimWorld.Passion> EdB.PrepareCarefully.CustomPawn::currentPassions
		// /* 0x00028F37 1107         */ IL_0197: ldloc.s   V_7
		// /* 0x00028F39 7B6605000A   */ IL_0199: ldfld     class ['Assembly-CSharp']RimWorld.SkillDef ['Assembly-CSharp']RimWorld.SkillRecord::def
		// /* 0x00028F3E 6FE600000A   */ IL_019E: callvirt  instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<class ['Assembly-CSharp']RimWorld.SkillDef, valuetype ['Assembly-CSharp']RimWorld.Passion>::get_Item(!0)
		// /* 0x00028F43 1309         */ IL_01A3: stloc.s   V_9
		// ----------------------------------------------------------------------------------------------------------------------------------------------- 
		// /* 0x00028F45 1109         */ IL_01A5: ldloc.s   V_9
		// /* 0x00028F47 17           */ IL_01A7: ldc.i4.1
		// /* 0x00028F48 3309         */ IL_01A8: bne.un.s  IL_01B3
		// 
		// /* 0x00028F4A 7E62020004   */ IL_01AA: ldsfld    class [UnityEngine]UnityEngine.Texture2D EdB.PrepareCarefully.Textures::TexturePassionMinor
		// /* 0x00028F4F 130A         */ IL_01AF: stloc.s   V_10
		// /* 0x00028F51 2B15         */ IL_01B1: br.s      IL_01C8
		// 
		// /* 0x00028F53 1109         */ IL_01B3: ldloc.s   V_9
		// /* 0x00028F55 18           */ IL_01B5: ldc.i4.2
		// /* 0x00028F56 3309         */ IL_01B6: bne.un.s  IL_01C1
		// 
		// /* 0x00028F58 7E61020004   */ IL_01B8: ldsfld    class [UnityEngine]UnityEngine.Texture2D EdB.PrepareCarefully.Textures::TexturePassionMajor
		// /* 0x00028F5D 130A         */ IL_01BD: stloc.s   V_10
		// /* 0x00028F5F 2B07         */ IL_01BF: br.s      IL_01C8
		// 
		// /* 0x00028F61 7E69020004   */ IL_01C1: ldsfld    class [UnityEngine]UnityEngine.Texture2D EdB.PrepareCarefully.Textures::TexturePassionNone
		// /* 0x00028F66 130A         */ IL_01C6: stloc.s   V_10
		// 
		// /* 0x00028F68 286200000A   */ IL_01C8: call      valuetype [UnityEngine]UnityEngine.Color [UnityEngine]UnityEngine.Color::get_white()
		// /* 0x00028F6D 282D01000A   */ IL_01CD: call      void [UnityEngine]UnityEngine.GUI::set_color(valuetype [UnityEngine]UnityEngine.Color)
		// /* 0x00028F72 1105         */ IL_01D2: ldloc.s   V_5
		// /* 0x00028F74 110A         */ IL_01D4: ldloc.s   V_10
		// /* 0x00028F76 287301000A   */ IL_01D6: call      void [UnityEngine]UnityEngine.GUI::DrawTexture(valuetype [UnityEngine]UnityEngine.Rect, class [UnityEngine]UnityEngine.Texture)
		// -----------------------------------------------------------------------------------------------------------------------------------------------
		// /* 0x00028F7B 1105         */ IL_01DB: ldloc.s   V_5
		// /* 0x00028F7D 16           */ IL_01DD: ldc.i4.0
		// /* 0x00028F7E 287C01000A   */ IL_01DE: call      bool ['Assembly-CSharp']Verse.Widgets::ButtonInvisible(valuetype [UnityEngine]UnityEngine.Rect, bool)
		// /* 0x00028F83 2C2A         */ IL_01E3: brfalse.s IL_020F



		private const int startOffset = 5;
		private const int endOffset = -3;

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
						if (type.Name == "currentPassions")
						{
							startIndex = i + startOffset;
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
						if (type.Name == "ButtonInvisible")
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
				var loadPawn = new CodeInstruction(OpCodes.Ldloc_S, 9);
				var loadRect = new CodeInstruction(OpCodes.Ldloc_S, 5);
				
				var mi = AccessTools.Method(typeof(PatchPrepareCarefullyBase), nameof(PatchPrepareCarefullyBase.DrawInterest));
				var modCall = new CodeInstruction(OpCodes.Call, mi);

				codes[startIndex] = loadPawn;
				codes[startIndex + 1] = loadRect;
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
