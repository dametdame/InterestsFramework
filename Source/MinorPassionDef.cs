using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace DInterests
{
    class MinorPassionDef : InterestDef
    {
        public override float GetWeight(SkillRecord skill, Pawn pawn)
        {
            float level = skill.Level;
            if (level <= 9)
                return base.GetWeight(skill, pawn);
            return 100 - (float)Math.Pow(level, weightLevelExponent) * weightLevelFactor / 4.0f;
        }
    }
}
