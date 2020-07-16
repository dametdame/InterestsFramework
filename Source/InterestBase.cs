using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.IO;
using Verse.AI;

namespace DInterests
{
	[StaticConstructorOnStartup]
	public static class InterestBase
	{
		public static InterestList interestList = new InterestList();
		public static InterestList positiveInterestList;
		public static InterestList negativeInterestList;

		

		static InterestBase()
		{
			int defCount = DefDatabase<DInterests.InterestDef>.DefCount;
			Log.Message("DInterests, found " + defCount.ToString() + " PassionDefs");

			positiveInterestList = new InterestList(interestList.FindAll(x => x.GetValue() > 0 || x.ignoreTraits));
			negativeInterestList = new InterestList(interestList.FindAll(x => x.GetValue() <= 0 || x.ignoreTraits));

		}


		public static void DrawSkill(Rect position, SkillRecord sk)
		{
			Texture2D image = interestList[(int)sk.passion].GetTexture();
			if (image != null)
				GUI.DrawTexture(position, image);
		}

		public static string GetSkillDescription(SkillRecord sk)
		{
			return interestList[(int)sk.passion].GetDescription();
		}

		public static float GetInspirationFactor(int interest)
		{
			return interestList[interest].GetInspirationFactor();
		}

		public static float GetValue(int interest)
		{
			return interestList[interest].GetValue();
		}

		public static void DrawWorkBoxBackground(Passion passion, Rect rect)
		{
			//Log.Message(((int)passion).ToString());
			interestList[(int)passion].drawWorkBox(rect);
		}


		public static float SumWeights(InterestList interests, SkillRecord skill, Pawn pawn)
		{
			float sum = 0.0f;
			foreach (InterestDef passion in interests)
			{
				float result = passion.GetWeight(skill, pawn);
				sum += result;
			}
			return  sum;
		}

		public static InterestDef GetInterest(InterestList interests, SkillRecord skill, Pawn pawn)
		{
			float totalWeight = -1;
			float random = -1;
			InterestDef selected = null;
			try
			{
				totalWeight = SumWeights(interests, skill, pawn);
				if (totalWeight == 0)
					return interests[0]; 
				random = Rand.Range(0.0f, totalWeight);
				// random number generates is inclusive, so we make sure we don't hit the ends or errors result
				// this is a bit odd, but done so that we can do a strict < comparison below which
				// allows us to avoid the possibility of a 0-weight interest being chosen when totalweight > 0
				if(random == 0)
				{
					random = 0.0001f;
					Log.Message("Generated perfect 0 from random number generator, wow!");
				}
				if(random == totalWeight)
				{
					random -= 0.0001f;
					Log.Message("Generated perfect 1 from random number generator, wow!");
				}
				
				foreach (InterestDef passion in interests)
				{
					float weight = passion.GetWeight(skill, pawn);
					if (random < weight)
					{
						selected = passion;
						break;
					}

					random -= weight;
				}
				if (selected == null)
					throw new NullReferenceException("selected still null at end of loop");
				return selected;
			}
			catch (NullReferenceException e)
			{
				Log.Message(e.Message);
				Log.Message("level " + (float)skill.Level);
				Log.Message("Random " + random);
				Log.Message("TotalWeight " + totalWeight);
				foreach (InterestDef passion in interests)
				{
					Log.Message(passion.defName + " weight " + passion.GetWeight(skill, pawn));
				}
			}
			return null;
		}

		public static void InheritSkill(SkillRecord sr, Pawn mother, Pawn father)
		{
			if (mother == null)
			{
				sr.passion = father.skills.GetSkill(sr.def).passion;
			}
			else if (father == null)
			{
				sr.passion = mother.skills.GetSkill(sr.def).passion;
			}
			else
			{
				float coin = Rand.Value;
				if (coin < 0.5)
				{
					sr.passion = mother.skills.GetSkill(sr.def).passion;
				}
				else
				{
					sr.passion = father.skills.GetSkill(sr.def).passion;
				}
			}
		}

		public static void Inherit(Pawn pawn, Pawn mother, Pawn father, bool random)
		{
			/*if (!random)
				foreach (SkillRecord sr in pawn.skills.skills)
					InheritSkill(sr, mother, father);
			
			else*/
				GenerateInterests(pawn, mother, father, true);
		}

		public static void GenerateInterests(Pawn pawn, Pawn father = null, Pawn mother = null, bool inherit = false)
		{
			List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;

			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				SkillDef skillDef = allDefsListForReading[i];
				SkillRecord skill = pawn.skills.GetSkill(skillDef);

				if (!skill.TotallyDisabled)
				{
					if (inherit)
					{
						float coin = Rand.Value;
						if (coin < 0.5)
						{
							InheritSkill(skill, mother, father);
							continue;
						}
					}

					bool forbidPositive = NoPositiveInterests(pawn, skillDef);
					bool requirePositive = ForcePositiveInterest(pawn, skillDef);

					InterestDef selected = null;
					if (!forbidPositive) // conflicts take precedence over requirements
					{
						if (!requirePositive) // all interests are possible
						{
							selected = GetInterest(interestList, skill, pawn);
						}
						else // only positives
						{
							if (positiveInterestList.Count == 0)
								selected = interestList.GetDefault();
							else
								selected = GetInterest(positiveInterestList, skill, pawn);
						}
					}
					else // no positives
					{
						if (negativeInterestList.Count == 0)
							selected = interestList.GetDefault();
						else
							selected = GetInterest(negativeInterestList, skill, pawn);
					}
					skill.passion = (Passion)interestList.IndexOf(selected);
				}
			}
		}

		public static bool NoPositiveInterests(Pawn pawn, SkillDef skillDef)
		{
			foreach (Trait trait in pawn.story.traits.allTraits)
			{
				if (trait.def.ConflictsWithPassion(skillDef))
					return true;
			}
			return false;
		}
		public static bool ForcePositiveInterest(Pawn pawn, SkillDef skillDef)
		{
			foreach (Trait trait in pawn.story.traits.allTraits)
			{
				if (trait.def.RequiresPassion(skillDef))
					return true;
			}
			return false;
		}

		public static float LearnRateFactor(Passion passion)
		{
			return interestList[(int)passion].GetLearnRate();
		}

		public static void RecordMasterTale(SkillRecord sr, Pawn p)
		{
			interestList[(int)sr.passion].RecordMasterTale(sr, p);
		}

		public static void HandleLearn(ref float xp, ref bool direct, SkillRecord sr, Pawn pawn)
		{
			interestList[(int)sr.passion].HandleLearn(ref xp, sr, pawn, ref direct);
		}

		public static void HandleSkillTick(Pawn_SkillTracker pst, Pawn pawn)
		{
			if (!pawn.IsColonist)
				return;
			if (interestList.tickHandlers.Count == 0)
				return;
			if (pawn.IsHashIntervalTick(200))
			{
				List<SkillRecord> handleSkills = pst.skills.Where(x => interestList.tickHandlers.ContainsKey((int)x.passion)).ToList();
				foreach (SkillRecord sr in handleSkills)
					interestList[(int)sr.passion].HandleTick(sr, pawn);
			}
			return;
		}

		public static List<InterestDef> getInspiringDefs()
		{
			return interestList.FindAll(x => x.inspires);
		}

		public static InterestDef getBestInspirer(List<int> l)
		{
			return interestList[l.MaxBy(x => interestList[x].inspirationChanceMultiplier)];
		}

		// runs every 100 ticks, postfix to stub StartInspirationMTBDays patch
		public static void HandleInspirationMTB(ref float result, InspirationHandler ih)
		{
			if (interestList.inspirers.Count == 0)
				return;

			if (ih.pawn.needs.mood == null) // result = -1, no mood value yet, so return
				return;

			// go through each skill this pawn has, see if they have an inspiring passion
			Pawn_SkillTracker pst = ih.pawn.skills;
			if (pst == null)
				return;
			if (pst.skills == null)
				return;

			// find any inspiring interests this pawn has and get their indices
			List<int> inspiringList = pst.skills.Select(s => (int)s.passion).Where(x => interestList[x].inspires).ToList();

			if (inspiringList.Count < 1) // no pawn skill with inspiring passion
				return;

			InterestDef interest = getBestInspirer(inspiringList);

			result = interest.MTBDays(ih);
		}

		public static SkillDef GetActiveSkill(Pawn pawn)
		{
			Pawn_JobTracker jt = pawn.jobs;
			if (pawn.skills == null)
				return null;
			if (jt == null)
				return null;
			JobDriver curDriver = jt.curDriver;
			if (curDriver == null)
				return null;
			return curDriver.ActiveSkill;
		}

		public static void UpdateMapwideEffect(Pawn p, int passion, Pawn instigator)
		{
			interestList[passion].UpdatePersistentWorkEffect(p, instigator);
		}

		public static int GetBetterPassion(int passion)
		{
			float value = interestList[passion].GetValue();
			int found;
			if (value <= 0 && positiveInterestList.Count > 0) {
				found = interestList.IndexOf(positiveInterestList.MinBy(x => x.GetValue()));
				return found;
			}
			List<InterestDef> foundList = interestList.FindAll(x => x.GetValue() > value);
			if (foundList.Count == 0)
				return passion;
			found = interestList.IndexOf(foundList.MinBy(y => y.GetValue()));
			return found;
		}

		public static int GetWorsePassion(int passion)
		{
			float value = interestList[passion].GetValue();
			int found;
			if (value > 0 && negativeInterestList.Count > 0)
			{
				found = interestList.IndexOf(negativeInterestList.MaxBy(x => x.GetValue()));
				return found;
			}
			List<InterestDef> foundList = interestList.FindAll(x => x.GetValue() < value);
			if (foundList.Count == 0)
				return passion;
			found = interestList.IndexOf(foundList.MaxBy(y => y.GetValue()));
			return found;
		}
	}

}
