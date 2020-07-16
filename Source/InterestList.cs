using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Noise;

namespace DInterests
{
    public class InterestList : List<InterestDef>
    {

        public Dictionary<int, InterestDef> mapwideWorkers;
        public Dictionary<int, InterestDef> inspirers;
        public Dictionary<int, InterestDef> tickHandlers;

        public InterestList() : base()
        {
            List<InterestDef> tmpList = new List<InterestDef>(DefDatabase<InterestDef>.AllDefsListForReading);
            if (tmpList.Count == 0)
                throw new System.ArgumentException("DInterests: Must have at least 1 PassionDef, failed to find any. Loading at least 3 is highly recommended.");
          
            // For compatability, try to make our first indices match the original enum
            InterestDef[] defaults = { InterestDefOf.DNoPassion, InterestDefOf.DMinorPassion, InterestDefOf.DMajorPassion }; // 0, 1, 2
            foreach (InterestDef def in defaults)
            {
                var find = tmpList.FindAll(x => x == def);
                if (!find.NullOrEmpty())
                {
                    AddRange(find);
                    tmpList.Remove(find[0]);
                }
            }
            tmpList.SortBy(x => x.priority);
            AddRange(tmpList);

            this.process();
        }

        public InterestList(List<InterestDef> p) : base(p) { this.process(); }

        private void process()
        {
            mapwideWorkers = this.Where(x => x.mapwideWorkImpacter).ToDictionary(i => this.FindIndex(m => m==i));
            inspirers = this.Where(x => x.inspires).ToDictionary(i => this.FindIndex(m => m == i));
            tickHandlers = this.Where(x => x.handlesTicks).ToDictionary(i => this.FindIndex(m => m == i));
        }

        public int this[string s]
        {
            get
            {
                return this.FindIndex(x => x.defName == s);
            }
        }

        public InterestDef GetDefault()
        {
            return this.MinBy(x => x.priority);
        }

        public int GetDefaultIndex()
        {
            return IndexOf(GetDefault());
        }
    }
}
