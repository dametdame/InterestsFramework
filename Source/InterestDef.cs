using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace DInterests
{
    /*
     * virtual Texture2D GetTexture()   : Gets the icon for this interest
     * virtual string GetDescription() : Gets the description for this interest
     * virtual float GetInspirationFactor() : Gets the inspiration weight
     * virtual float GetValue()
     * virtual void drawWorkBox(Rect rect)C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\Interests Framework\Source\ThoughtWorker_Interests.cs
     * virtual bool IsDisabled(SkillRecord skill) : checks whether the given skill is eligible for this passion
     * virtual bool IsValidForPawn(Pawn pawn) : checks whether the given pawn can have this interest in the given skill
     * virtual float GetWeight(SkillRecord skill, Pawn pawn, int lvl = -1)
     * virtual float GetLearnRate()
     * virtual void RecordMasterTale(SkillRecord sr, Pawn p)
     * virtual void HandleLearn(float xp, SkillRecord sk, Pawn Pawn, bool direct = false) : Called before learn
     * virtual void HandleTick(SkillRecord sr, Pawn pawn) : called every 200 ticks
     * virtual int InterestCountForPawn(Pawn pawn)
     * virtual float MTBDays(InspirationHandler ih
     * virtual void UpdatePersistentWorkEffect(Pawn pawn) : called every 200 ticks if this is marked as a mapwideWorkImpacter
     *                                                      and ANY colonist is doing work associated with a skill that ANY
     *                                                      colonist on the map has this interest in.
     */

    public class InterestDef : Def
    {
        

        public float learnFactor = 0.35f;
        public float forgetFactor = 1.0f;
        public string texPath = null;
        public string texPathGrey = null;
        public float maxPerPawn = 100;          // 100 is effectively unlimited
       

        public float priority = float.MinValue; // used to determine whether to draw this priority in the work tab; 
                                                // one InterestDef should have a very low value to serve as "default,"
                                                // highest priority passion of relevant skills will be drawn in the work tab
                                                // also determines what order interests will appear in Prepare Carefully

        private float value = float.MinValue;   // used to determine which passion is the "best" for purposes of the colonist preparation tab
                                                // also describes whether an interest is good (>0), neutral (==0), or bad(<0)
                                                // default is 100*learnFactor - 35 (0 for 0.35 learnfactor)
                                                // highest value passion among those colonists with = skills will be drawn in prep screen
                                                // personality traits will prevent or require passions based on their good/neutral/bad-ness

        public bool canAppear = true;
        public bool ignoreTraits = false;
        public float weight = 8.8f;             // default formula = weight + weightLevelFactor * weightLevelExponent^level + weightLevelOffset*level;
        public float weightLevelExponent = 1.0f;
        public float weightLevelFactor = 1.0f;
        public float weightLevelOffset = 0.0f;

        public float inspirationFactor = 1.0f;  // determines how likely the associated skill is to be chosen for inspiration if an inspiration occurs
                                                // note: active even if inspires = false, but recommend you set high for inspires = true

        public bool inspires = false;
        public float inspirationMoodThreshold = 0.5f;
        public float inspirationChanceMultiplier = 3.0f;

        public bool mapwideWorkImpacter = false;
        public bool handlesTicks = false;

        public List<SkillDef> ineligibleSkills = new List<SkillDef>();

        public static bool operator <(InterestDef a, InterestDef b) => a.priority < b.priority;
        public static bool operator >(InterestDef a, InterestDef b) => a.priority > b.priority;

        // get the texture for this interest's icon
        public virtual Texture2D GetTexture()
        {
            if (texPath == null)
                return null;
            return ContentFinder<Texture2D>.Get(texPath, true);
        }

        // get the description from Languages/English/Keyed/Keyed.xml
        public virtual string GetDescription()
        {
            var sb = new StringBuilder();
            var descToFind = defName + ".description";
            sb.Append(descToFind.Translate(learnFactor.ToStringPercent("F0")));
            return sb.ToString();
        }

        // get inspiration weight, see abvove
        public virtual float GetInspirationFactor()
        {
            return inspirationFactor;
        }

        // get value, see above
        public virtual float GetValue()
        {
            if (value != float.MinValue)
                return value;
            return learnFactor * 100 - 35;
        }

        // draw the icon for this passion in the work tab
        public virtual void drawWorkBox(Rect rect)
        {
            string drawTex = texPath;
            if (drawTex == null)
                return;
            var tex = ContentFinder<Texture2D>.Get(drawTex, true);
            GUI.color = new Color(1f, 1f, 1f, 1.0f);
            Rect position = rect;
            var xoffset = rect.width/4.0f;
            var yoffset = rect.height/8.0f;
            position.xMin = rect.center.x + xoffset/2.0f;
            position.yMin = rect.center.y + yoffset/2.0f;
            position.xMax += xoffset;
            position.yMax += yoffset;
            GUI.DrawTexture(position, tex);
        }

        // checks whether the given skill is eligible for this passion
        public virtual bool IsDisabled(SkillDef def)
        {
            if (def == null)
                return false;
            if (ineligibleSkills.IndexOf(def) > -1)
                return true;
            return false;
        }

        // checks whether the given pawn can have this interest in the given skill
        public virtual bool IsValidForPawn(Pawn pawn, SkillDef def)
        {
            if (!this.canAppear)
                return false;
            if (InterestCountForPawn(pawn) >= maxPerPawn)
                return false;
            return !IsDisabled(def);
        }

        // calculate weight for purposes of determining chance for pawn to have this skill
        // lvl parameter is used for testing
        public virtual float GetWeight(SkillRecord skill, Pawn pawn)
        {
            if (skill != null && !IsValidForPawn(pawn, skill.def))
                return 0;

            float level = skill.Level;
            float levelTerm = weightLevelFactor * (float)Math.Pow(weightLevelExponent, level);
            return Mathf.Max(0, weight + levelTerm + weightLevelOffset * level);
        }

        // Patch_LearnRateFactor_Prefix replaces original LearnRateFactor function
        // Called as a replacement
        public virtual float GetLearnRate()
        {
            return learnFactor;
        }

        // called in Learn when pawn gains skill level of 14
        public virtual void RecordMasterTale(SkillRecord sr, Pawn p)
        {
            if (this.GetValue() <= 0)
            {
                TaleRecorder.RecordTale(TaleDefOf.GainedMasterSkillWithoutPassion, new object[]
                {
                        p,
                        sr.def
                });
            }
            else
            {
                TaleRecorder.RecordTale(TaleDefOf.GainedMasterSkillWithPassion, new object[]
                {
                        p,
                        sr.def
                });
            }
        }

        // called before a skill's XP is updated 
        // xp < 0 : interval ticked to decrease xp over time, may be modified (or nulled) by Mad Skills or ot her mods
        // by default will modify by forgetFactor
        public virtual void HandleLearn(ref float xp, SkillRecord sk, Pawn pawn, ref bool direct)
        {
            if (xp < 0 && !direct)
                xp *= forgetFactor;
            return;
        }

        // occurs after SkillsTick is called, but only when IsHashIntervalTick(200)
        // this is when SkillsTick normally processes
        public virtual void HandleTick(SkillRecord sr, Pawn pawn)
        {
            return;
        }

        public virtual int InterestCountForPawn(Pawn pawn)
        {
            if (pawn == null)
                return 0;
            int index = InterestBase.interestList[defName];
            int count = 0;
            foreach (SkillRecord sr in pawn.skills.skills)
            {
                if ((int)sr.passion == index)
                    ++count;
            }
            return count;
        }

        // this doesn't do anything by default
        public virtual void UpdatePersistentWorkEffect(Pawn pawn, Pawn instigator)
        {
            return;
        }

        // inspiration function, see below
        public virtual float MTBDays(InspirationHandler ih)
        {
            float curLevel = ih.pawn.needs.mood.CurLevel;
            if (curLevel < inspirationMoodThreshold)
                return -1f;
            return GenMath.LerpDouble(inspirationMoodThreshold, 1f, 210f, 10f, curLevel) / inspirationChanceMultiplier;
        }

        // LerpDouble returns 210 + (10 - 210) * (curLevel - moodThreshold) / (1 - moodThreshold)
        // this function ranges from 10 to 210
        // lower values will give us better inspiration results
        // changing the mood threshold does NOT affect minimum or maximum
        // mood threshold of 0 will get you an "average" of 1/2 result (2x chance of firing) for mood = [0.5, 1] vs 0.5 threshold
        //
        // 2500 ticks per in-game hour = 25 checks per hour
        //
        // Next function called is (Rand.MTBEventOccurs(<result of this function>, 60000f, 100f)) where
        // num = 10 / (result * 60000)
        // num2 = 1
        // if num < 0.0001, the following loop is skipped, but this occurs at result = 1.66666666667 which is impossible
        // then we enter a while loop with num * 8 and num2 / 8, checking if a rand value > num2 each time and returning false if so
        // finally, we return true if a random value < num
        //
        // for result = 2 (not normally possible): num = 0.00008333
        // num1 = 0.000666640, num2 = 0.125
        // (0.125)(0.000666640) = 0.000083330 = 0.0083330% chance of firing per check 
        // 0.0020811 = 0.20811% chance per hour
        // 0.048769095 = 4.8769095% chance per day
        // 0.527622 = 52.7622% chance per season
        // 0.9502 = 95.02%  chance per year
        //
        // for result = 10: num = 0.00001666666 = 1/60000
        // num1 = 0.00013333333, num2 = 0.125
        // (0.125)(0.00013333333) = 0.00001666666 = 0.001666666% chance of firing per check
        // 0.0004165831 = 0.04165831% chance per hour
        // 0.009950242 = 0.9950242% chance per day
        // 0.13929301 = 13.929% chance per season
        // 0.45119 = 45.119% chance per year
        //
        // for result = 210: num = 7.93650794e-7
        // num1 = 6.3492063492e-6, num2 = 0.125; num1 = 0.00005079365, num2 = 0.015625; num1 = 0.0004063492063, num2 = 0.001953125
        // (0.001953125)(0.0004063492063) = 0.000000793650793 = 0.0000793650793% chance of firing per check
        // 0.00001984108086 = 0.001984108086% chance per hour
        // 0.00047607 = 0.047607% chance per day
        // 0.007117 = 0.7117% chance per season
        // 0.028165 = 2.8165% chance per year
        //
        // result = 10 is 21 times more likely to fire than result = 210
        // result = 2 is 5 times more likely to fire than result = 10
        //
        // dividing result by x will increase fire chance per check by a factor of x
    }
}
