using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{

    [Serializable()]
    public class Reol
    {
        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private string _ReolNumber;
        public string ReolNumber
        {
            get
            {
                return _ReolNumber;
            }
            set
            {
                _ReolNumber = value;
            }
        }

        private string _Description;

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        private string _Comment;

        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
            }
        }

        public string DescriptiveName
        {
            get
            {
                return Description + " (" + ReolId + ")";
            }
        }

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

        private string _Kommune;

        public string Kommune
        {
            get
            {
                return _Kommune;
            }
            set
            {
                _Kommune = value;
            }
        }

        private string _KommuneFullDistribusjon;

        public string KommuneFullDistribusjon
        {
            get
            {
                return _KommuneFullDistribusjon;
            }
            set
            {
                _KommuneFullDistribusjon = value;
            }
        }

        private string _FylkeId;

        public string FylkeId
        {
            get
            {
                return _FylkeId;
            }
            set
            {
                _FylkeId = value;
            }
        }

        private string _Fylke;

        public string Fylke
        {
            get
            {
                return _Fylke;
            }
            set
            {
                _Fylke = value;
            }
        }

        private string _TeamNumber;

        public string TeamNumber
        {
            get
            {
                return _TeamNumber;
            }
            set
            {
                _TeamNumber = value;
            }
        }

        private string _TeamName;

        public string TeamName
        {
            get
            {
                return _TeamName;
            }
            set
            {
                _TeamName = value;
            }
        }

        private string _PostalZone;

        public string PostalZone
        {
            get
            {
                return _PostalZone;
            }
            set
            {
                _PostalZone = value;
            }
        }

        private string _PostalArea;

        public string PostalArea
        {
            get
            {
                return _PostalArea;
            }
            set
            {
                _PostalArea = value;
            }
        }

        private string _SegmentId;

        public string SegmentId
        {
            get
            {
                return _SegmentId;
            }
            set
            {
                _SegmentId = value;
            }
        }

        private AntallInformation _Antall;

        public AntallInformation Antall
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

        private AvisDekningCollection _AvisDeknings;
        public AvisDekningCollection AvisDeknings
        {
            get
            {
                return _AvisDeknings;
            }
            set
            {
                _AvisDeknings = value;
            }
        }

        private int _PrisSone;

        public int PrisSone
        {
            get
            {
                return _PrisSone;
            }
            set
            {
                _PrisSone = value;
            }
        }

        private string _RuteType;

        public string RuteType
        {
            get
            {
                return _RuteType;
            }
            set
            {
                _RuteType = value;
            }
        }

        private string _PostkontorNavn;

        public string PostkontorNavn
        {
            get
            {
                return _PostkontorNavn;
            }
            set
            {
                _PostkontorNavn = value;
            }
        }

        private string _PrsEnhetsId;

        public string PrsEnhetsId
        {
            get
            {
                return _PrsEnhetsId;
            }
            set
            {
                _PrsEnhetsId = value;
            }
        }

        private string _PrsName;

        public string PrsName
        {
            get
            {
                return _PrsName;
            }
            set
            {
                _PrsName = value;
            }
        }

        private string _PrsDescription;

        public string PrsDescription
        {
            get
            {
                return _PrsDescription;
            }
            set
            {
                _PrsDescription = value;
            }
        }
        // Added for RDF
        private string _Frequency;
        public string Frequency
        {
            get
            {
                return _Frequency;
            }
            set
            {
                _Frequency = value;
            }
        }
        private string _SondagFlag;

        public string SondagFlag
        {
            get
            {
                return _SondagFlag;
            }
            set
            {
                _SondagFlag = value;
            }
        }


        /// <summary>
        /// Fill index data
        /// </summary>
        public Dictionary<string, double> IndexData { get; set; }
    }
}
