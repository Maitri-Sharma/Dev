using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class Bydel
    {
        private string _BydelID;
        public string BydelID
        {
            get
            {
                return _BydelID;
            }
            set
            {
                _BydelID = value;
            }
        }

        private string _Bydel;
        public string By_del
        {
            get
            {
                return _Bydel;
            }
            set
            {
                _Bydel = value;
            }
        }

        private string _By;
        public string By
        {
            get
            {
                return _By;
            }
            set
            {
                _By = value;
            }
        }

        public Bydel()
        {
        }

        public Bydel(string bydelid, string bydel, string by)
        {
            this.BydelID = bydelid;
            this.By_del = bydel;
            this.By = by;
        }
    }

    public class BydelCollection : System.Collections.Generic.List<Bydel>
    {

        // Gets all distinct cities for the collection
        public string[] GetBys()
        {
            string byer = "";
            string comma = "";
            foreach (Bydel b in this)
            {
                if (Strings.InStr(byer, b.By) == 0)
                {
                    byer = byer + comma + b.By;
                    comma = ",";
                }
            }
            return byer.Split(",");
        }

        public BydelCollection GetBydelerIBy(string by)
        {
            BydelCollection subColl = new BydelCollection();
            foreach (Bydel bydel in this)
            {
                if (bydel.By == by)
                    subColl.Add(bydel);
            }
            return subColl;
        }

        public Bydel GetBydelVedId(string id)
        {
            foreach (Bydel bydel in this)
            {
                if (bydel.BydelID == id)
                    return bydel;
            }
            return null;
        }

        public bool ContainsBydel(string id)
        {
            foreach (Bydel bydel in this)
            {
                if (bydel.BydelID == id)
                    return true;
            }
            return false;
        }
    }
}
