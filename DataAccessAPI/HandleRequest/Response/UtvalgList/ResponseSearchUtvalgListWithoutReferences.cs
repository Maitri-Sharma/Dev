using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.UtvalgList
{
    public class ResponseSearchUtvalgListWithoutReferences
    {
        private int _ListId;

        public int ListId
        {
            get
            {
                return _ListId;
            }
            set
            {
                _ListId = value;
            }
        }


        private List<UtvalgModification> _Modifications;
        public List<UtvalgModification> Modifications
        {
            get
            {
                return _Modifications;
            }
            set
            {
                _Modifications = value;
            }
        }

        private string _Logo;

        public string Logo
        {
            get
            {
                return _Logo;
            }
            set
            {
                _Logo = value;
            }
        }


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

        private string _KundeNavn;

        public string KundeNavn
        {
            get
            {
                return _KundeNavn;
            }
            set
            {
                _KundeNavn = value;
            }
        }

        private string _OrdreReferanse;
        public string OrdreReferanse
        {
            get
            {
                return _OrdreReferanse; // Referanse til en ordre eller et tilbud. Avhengig av gitt type
            }
            set
            {
                _OrdreReferanse = value;
            }
        }

        private PumaEnum.OrdreType _OrdreType;
        public PumaEnum.OrdreType OrdreType
        {
            get
            {
                return _OrdreType; // Type Ordre
            }
            set
            {
                _OrdreType = value;
            }
        }

        private PumaEnum.OrdreStatus _OrdreStatus;
        public PumaEnum.OrdreStatus OrdreStatus
        {
            get
            {
                return _OrdreStatus; // Status på en ordre eller tilbud
            }
            set
            {
                _OrdreStatus = value;
            }
        }

        private string _KundeNummer;
        public string KundeNummer
        {
            get
            {
                return _KundeNummer;
            }
            set
            {
                _KundeNummer = value;
            }
        }

        private DateTime _InnleveringsDato;
        public DateTime InnleveringsDato
        {
            get
            {
                return _InnleveringsDato; // Dato for når en ordre skal innleveres
            }
            set
            {
                _InnleveringsDato = value;
            }
        }

        /// <summary>
        ///     ''' Evt avtalenummer tilknyttet utvalg. Settes og benyttes av Ordre
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private int _Avtalenummer;
        public Nullable<int> Avtalenummer
        {
            get
            {
                return _Avtalenummer;
            }
            set
            {
                _Avtalenummer = (int)value;
            }
        }

        /// <summary>
        ///     ''' NB!!: Benyttes kun av og er kun implementert for integrasjon mot Ordre
        ///     ''' Har ellers ingen verdi satt.
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private DateTime _SistOppdatert;
        public DateTime SistOppdatert
        {
            get
            {
                return _SistOppdatert; // Dato for når innholdet i lista sist ble oppdatert
            }
            set
            {
                _SistOppdatert = value;
            }
        }

        /// <summary>
        ///     ''' NB!!: Benyttes kun av og er kun implementert for integrasjon mot Ordre.
        ///     ''' Har ellers ingen verdi satt.
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private string _SistEndretAv;
        public string SistEndretAv
        {
            get
            {
                return _SistEndretAv; // Hvem endret innholdet i lista sist
            }
            set
            {
                _SistEndretAv = value;
            }
        }

        private long _AntallWhenLastSaved = 0;
        public long AntallWhenLastSaved
        {
            get
            {
                return _AntallWhenLastSaved;
            }
            set
            {
                _AntallWhenLastSaved = value;
            }
        }

        private int _Weight;
        public int Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                _Weight = value;
            }
        }

        private PumaEnum.DistributionType _DistributionType;
        public PumaEnum.DistributionType DistributionType
        {
            get
            {
                return _DistributionType;
            }
            set
            {
                _DistributionType = value;
            }
        }

        private DateTime _DistributionDate;
        public DateTime DistributionDate
        {
            get
            {
                return _DistributionDate;
            }
            set
            {
                _DistributionDate = value;
            }
        }

        private bool _IsBasis;
        public bool IsBasis
        {
            get
            {
                return _IsBasis;
            }
            set
            {
                _IsBasis = value;
            }
        }

        private int _BasedOn;
        public int BasedOn
        {
            get
            {
                return _BasedOn;
            }
            set
            {
                _BasedOn = value;
            }
        }

        //private string _BasedOnName;
        //public string BasedOnName
        //{
        //    get
        //    {
        //        return _BasedOnName;
        //    }
        //    set
        //    {
        //        _BasedOnName = value;
        //    }
        //}

        private int _WasBasedOn;
        public int WasBasedOn
        {
            get
            {
                return _WasBasedOn;
            }
            set
            {
                _WasBasedOn = value;
            }
        }

        //private string _WasBasedOnName;
        //public string WasBasedOnName
        //{
        //    get
        //    {
        //        return _WasBasedOnName;
        //    }
        //    set
        //    {
        //        _WasBasedOnName = value;
        //    }
        //}

        private bool _AllowDouble;
        public bool AllowDouble
        {
            get
            {
                return _AllowDouble;
            }
            set
            {
                _AllowDouble = value;
            }
        }

        private List<CampaignDescription> _ListsBasedOnMe;
        public List<CampaignDescription> ListsBasedOnMe
        {
            get
            {
                return _ListsBasedOnMe;
            }
            set
            {
                _ListsBasedOnMe = value;
            }
        }

        //private List<Utvalg> _MemberUtvalgs = new List<Utvalg>();
        //public List<Utvalg> MemberUtvalgs
        //{
        //    get
        //    {
        //        return _MemberUtvalgs;
        //    }
        //    set
        //    {
        //        _MemberUtvalgs = value;
        //    }
        //}

        //private List<UtvalgList> _MemberLists = new List<UtvalgList>();
        //public List<UtvalgList> MemberLists
        //{
        //    get
        //    {
        //        return _MemberLists;
        //    }
        //    set
        //    {
        //        _MemberLists = value;
        //    }
        //}

        public long Antall
        {
            get;
            //{
            //return CalculateAntall();
            //}
            set;
            // {
            // _Antall = CalculateAntall();
            // }
        }

        private double _thickness;
        public double Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
            }
        }

    }
}
