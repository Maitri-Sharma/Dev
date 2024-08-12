using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class Fylke
    {
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

        public Fylke()
        {
        }

        public Fylke(string id, string name)
        {
            FylkeID = id;
            FylkeName = name;
        }
    }

    public class FylkeCollection : System.Collections.Generic.List<Fylke>
    {
    }
}
