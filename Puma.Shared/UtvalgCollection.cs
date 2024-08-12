using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgCollection : List<Utvalg>
    {
        public UtvalgCollection()
        {
        }

        public UtvalgCollection(Utvalg u)
        {
            this.Add(u);
        }

        public bool ContainsUtvalg(int utvalgId)
        {
            foreach (Utvalg u in this)
            {
                if (u.UtvalgId == utvalgId)
                    return true;
            }
            return false;
        }

        public bool ContainsUtvalgBasedOnDemOrSeg()
        {
            bool demOrSeg = false;
            if (this.Count > 0)
            {
                foreach (Utvalg utv in this)
                {
                    if (utv.IsDemOrSeg())
                    {
                        demOrSeg = true;
                        break;
                    }
                }
            }
            return demOrSeg;
        }
    }
}
