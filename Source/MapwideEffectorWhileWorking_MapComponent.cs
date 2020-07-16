using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DInterests;
using RimWorld;
using Verse;
using Verse.AI;

namespace DInterestsCallings
{
    class MapwideEffectorWhileWorking_MapComponent : MapComponent
    {

        private int updateTicks = 0;

        //List<SkillRecord> skillsAffected = new List<SkillRecord>();
        Dictionary<SkillRecord, Pawn> skillsAffected = new Dictionary<SkillRecord, Pawn>();

        public MapwideEffectorWhileWorking_MapComponent(Map map) : base(map)
        { }

        public override void MapComponentTick()
        {
            // get interests that give joy, or return if none
            if (InterestBase.interestList.mapwideWorkers.Count == 0)
                return;

            updateTicks++;
            if (updateTicks % 200 != 0)
                return;
            updateTicks = 0;

            // find all skills for which a colonist with a skillaffector exists
            List<Pawn> colonists = map.mapPawns.FreeColonistsSpawned;

            skillsAffected.Clear();
            List<int> passions = InterestBase.interestList.mapwideWorkers.Keys.ToList();
            skillsAffected = colonists.SelectMany(x => x.skills.skills, (colonist, skillrecord) => new KeyValuePair<SkillRecord, Pawn>(skillrecord, colonist)).ToDictionary(x => x.Key, x=> x.Value);
            //skillsAffected = colonists.SelectMany(c => c.skills.skills).Where(s => passions.Contains((int)s.passion)).ToList();

            // go through every colonist, see if they're working on the affected skill, and affect them
            
            foreach (Pawn colonist in colonists)
            {
                SkillDef active = InterestBase.GetActiveSkill(colonist);
                if (active == null)
                    continue;

                //IEnumerable<SkillRecord> activeInterests = skillsAffected.Where(s => s.def == active);
                IEnumerable<KeyValuePair<SkillRecord, Pawn>> activeInterests = skillsAffected.Where(s => s.Key.def == active);
                foreach (var skill in activeInterests ?? Enumerable.Empty <KeyValuePair <SkillRecord, Pawn>> ())
                    InterestBase.UpdateMapwideEffect(colonist, (int)skill.Key.passion, skill.Value);
            }
        }

        public override void ExposeData()
        {

            base.ExposeData();
        }
    }
}
