using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class ReolerKommune
    {
        private long _ReolId;
        public long ReolId
        {
            get
            {
                return _ReolId;
            }
            set
            {
                _ReolId = value;
            }
        }

        private string _KommuneId;
        public string KommuneId
        {
            get
            {
                return _KommuneId;
            }
            set
            {
                _KommuneId = value;
            }
        }


        private int _HH;
        public int HH
        {
            get
            {
                return _HH;
            }
            set
            {
                _HH = value;
            }
        }


        private int _ER;
        public int ER
        {
            get
            {
                return _ER;
            }
            set
            {
                _ER = value;
            }
        }


        private int _GB;
        public int GB
        {
            get
            {
                return _GB;
            }
            set
            {
                _GB = value;
            }
        }


        private int _VH;
        public int VH
        {
            get
            {
                return _VH;
            }
            set
            {
                _VH = value;
            }
        }


        private int _HH_RES;
        public int HH_RES
        {
            get
            {
                return _HH_RES;
            }
            set
            {
                _HH_RES = value;
            }
        }

        private int _ER_RES;
        public int ER_RES
        {
            get
            {
                return _ER_RES;
            }
            set
            {
                _ER_RES = value;
            }
        }

        private int _GB_RES;
        public int GB_RES
        {
            get
            {
                return _GB_RES;
            }
            set
            {
                _GB_RES = value;
            }
        }
    }


    // Collection of ReolerKommune
    public class ReolerKommuneCollection : System.Collections.Generic.List<ReolerKommune>
    {
    }


    // Key for caching
    public class ReolerKommuneKey
    {
        public long ReolId;
        public string KommuneId;

        public ReolerKommuneKey(long reolId, string kommuneId)
        {
            this.ReolId = reolId;
            this.KommuneId = kommuneId;
        }

        public override bool Equals(object obj)
        {
            ReolerKommuneKey other = (ReolerKommuneKey)obj;
            return ReolId == other.ReolId & KommuneId.Equals(other.KommuneId);
        }

        public override int GetHashCode()
        {
            return ReolId.GetHashCode();
        }
    }

}
