using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgsListCollection : List<UtvalgList>
    {
        public bool containsID(UtvalgList list)
        {
            foreach (UtvalgList utvList in this)
            {
                if (list.ListId == utvList.ListId)
                    return true;
            }
            return false;
        }

        public bool HasReferenceToUtvalg(int utvalgId)
        {
            foreach (UtvalgList list in this)
            {
                if (list.HasUtvalgAsDescendant(utvalgId))
                    return true;
            }

            return false;
        }

        public Utvalg GetUtvalg(int utvalgId)
        {
            foreach (UtvalgList list in this)
            {
                if (list.HasUtvalgAsDescendant(utvalgId))
                    return list.GetUtvalgDescendant(utvalgId);
            }

            return null/* TODO Change to default(_) if this is not a reference type */;
        }

        public UtvalgList GetUtvalgList(int utvalgListId)
        {
            foreach (UtvalgList list in this)
            {
                if (list.ListId == utvalgListId)
                    return list;
                if (list.HasUtvalgListAsDescendant(utvalgListId))
                    return list.GetUtvalgListDescendant(utvalgListId);
            }

            return null/* TODO Change to default(_) if this is not a reference type */;
        }

        public bool HasReferenceToUtvalgList(int utvalgListId)
        {
            foreach (UtvalgList list in this)
            {
                if (list.ListId == utvalgListId)
                    return true;
                if (list.HasUtvalgListAsDescendant(utvalgListId))
                    return true;
            }

            return false;
        }
    }
}
