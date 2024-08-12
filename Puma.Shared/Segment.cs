using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class Segmentfordeling
    {
        private string _SegmentID;
        public string SegmentID
        {
            get
            {
                return _SegmentID;
            }
            set
            {
                _SegmentID = value;
            }
        }

        private int _Antall;
        public int Antall
        {
            get
            {
                return _Antall;
            }
            set
            {
                _Antall = value;
            }
        }

        public Segmentfordeling()
        {
        }

        public Segmentfordeling(string id, int ant)
        {
            SegmentID = id;
            Antall = ant;
        }
    }

    public class SegmentfordelingCollection : System.Collections.Generic.List<Segmentfordeling>
    {
    }
}
