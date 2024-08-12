using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class Kommune
    {
        private string _KommuneID;
        public string KommuneID
        {
            get
            {
                return _KommuneID;
            }
            set
            {
                _KommuneID = value;
            }
        }

        private string _KommuneName;
        public string KommuneName
        {
            get
            {
                return _KommuneName;
            }
            set
            {
                _KommuneName = value;
            }
        }

        private string _FylkeID;
        public string FylkeID
        {
            get
            {
                return _FylkeID;
            }
            set
            {
                _FylkeID = value;
            }
        }

        private string _FylkeName;
        public string FylkeName
        {
            get
            {
                return _FylkeName;
            }
            set
            {
                _FylkeName = value;
            }
        }

        private bool _isKommuneNameUnique;

        public bool IsKommuneNameUnique
        {
            get
            {
                return _isKommuneNameUnique;
            }
            set
            {
                _isKommuneNameUnique = value;
            }
        }

        public string KommuneIfylke
        {
            get
            {
                if (IsKommuneNameUnique)
                    return KommuneName;

                return KommuneName + " i " + FylkeName;
            }
        }

        public Kommune()
        {
        }

        public Kommune(string id, string name)
        {
            KommuneID = id;
            KommuneName = name;
        }
    }

    public class KommuneCollection : System.Collections.Generic.List<Kommune>
    {
        public Kommune GetKommuneById(string kommuneId)
        {
            foreach (Kommune k in this)
            {
                if (k.KommuneID == kommuneId)
                    return k;
            }

            return null;
        }

        public void RemoveById(string kommuneId)
        {
            Kommune removeItem = null;

            foreach (Kommune k in this)
            {
                if (k.KommuneID == kommuneId)
                {
                    removeItem = k;
                    break;
                }
            }

            this.Remove(removeItem);
        }

        public bool HasKommune(string kommuneId)
        {
            return GetKommuneById(kommuneId) != null;
        }

        public FylkeTree GroupByFylkeKommune()
        {
            FylkeTree result = new FylkeTree();
            foreach (Kommune r in this)
                result.InsertByFylkeKommune(r);
            return result;
        }

        public void SortByFylkeKommune()
        {
            this.Sort(new KommuneComparerByFylkeKommune());
        }

        public List<string> KommuneIDs
        {
            get
            {
                List<string> result = new List<string>();

                foreach (Kommune k in this)
                    result.Add(k.KommuneID);

                return result;
            }
        }
    }

    public class KommuneComparerByFylkeKommune : Comparer<Kommune>
    {
        public override int Compare(Kommune x, Kommune y)
        {
            try
            {
                int result = x.FylkeName.CompareTo(y.FylkeName);
                if (result == 0)
                    result = x.KommuneName.CompareTo(y.KommuneName);
                // If result = 0 Then result = x.Description.CompareTo(y.Description)
                if (result == 0)
                    result = x.KommuneID.CompareTo(y.KommuneID);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }
    }
}
