using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Response.Utvalg
{
    public class ResponseSearchUtlvagDetailById
    {
        private int _UtvalgId;

        public int UtvalgId
        {
            get
            {
                return _UtvalgId;
            }
            set
            {
                _UtvalgId = value;
            }
        }


        private bool _Changed = false;
        public bool Changed
        {
            get
            {
                return _Changed;
            }
            set
            {
                _Changed = value;
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

        private string _ReolMapName;

        public string ReolMapName
        {
            get
            {
                return _ReolMapName;
            }
            set
            {
                _ReolMapName = value;
            }
        }

        private string _OldReolMapName;

        public string OldReolMapName
        {
            get
            {
                return _OldReolMapName;
            }
            set
            {
                _OldReolMapName = value;
            }
        }
        private bool _Skrivebeskyttet;
        public bool Skrivebeskyttet
        {
            get
            {
                return _Skrivebeskyttet;
            }
            set
            {
                _Skrivebeskyttet = value;
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

        private DistributionType _DistributionType;
        public DistributionType DistributionType
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

        private double _ArealAvvik;
        public double ArealAvvik
        {
            get
            {
                return _ArealAvvik;
            }
            set
            {
                _ArealAvvik = value;
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

        private string _BasedOnName;
        public string BasedOnName
        {
            get
            {
                return _BasedOnName;
            }
            set
            {
                _BasedOnName = value;
            }
        }

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

        private string _WasBasedOnName;
        public string WasBasedOnName
        {
            get
            {
                return _WasBasedOnName;
            }
            set
            {
                _WasBasedOnName = value;
            }
        }

        //private UtvalgList _List;

        //public UtvalgList List
        //{
        //    get
        //    {
        //        return _List;
        //    }
        //    set
        //    {
        //        if (value != null && value.MemberUtvalgs != null)
        //        {
        //            if (!value.MemberUtvalgs.Contains(this))
        //                value.MemberUtvalgs.Add(this);
        //        }
        //        else if (_List != null && value == null)
        //            _List.MemberUtvalgs.Remove(this);

        //        _List = value;
        //    }
        //}

        public string ListId { get; set; }
        private ReolCollection _ReolerBeforeRecreation = null/* TODO Change to default(_) if this is not a reference type */;

        public ReolCollection ReolerBeforeRecreation
        {
            get
            {
                return _ReolerBeforeRecreation;
            }
            set
            {
                _ReolerBeforeRecreation = value;
            }
        }

        private ReolCollection _Reoler = new ReolCollection();

        public ReolCollection Reoler
        {
            get
            {
                return _Reoler;
            }
            set
            {
                _Reoler = value;
            }
        }

        private UtvalgReceiverList _Receivers = new UtvalgReceiverList();
        public UtvalgReceiverList Receivers
        {
            get
            {
                return _Receivers;
            }
            set
            {
                _Receivers = value;
            }
        }

        public long AntallBeforeRecreation
        {
            get;set;
        }

        public int TotalAntall
        {
            get;set;
            
        }

        

        /// <summary>
        ///     ''' Referanse til en ordre eller et tilbud. Avhengig av gitt type
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private string _OrdreReferanse;
        public string OrdreReferanse
        {
            get
            {
                return _OrdreReferanse;
            }
            set
            {
                _OrdreReferanse = value;
            }
        }

        /// <summary>
        ///     ''' Type Ordre. Tilbud eller Ordre
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private OrdreType _OrdreType;
        public OrdreType OrdreType
        {
            get
            {
                return _OrdreType;
            }
            set
            {
                _OrdreType = value;
            }
        }

        /// <summary>
        ///     ''' Status på en ordre eller tilbud
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private OrdreStatus _OrdreStatus;
        public OrdreStatus OrdreStatus
        {
            get
            {
                return _OrdreStatus;
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


        /// <summary>
        ///     ''' Dato for når en ordre skal innleveres
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private DateTime _InnleveringsDato;
        public DateTime InnleveringsDato
        {
            get
            {
                return _InnleveringsDato;
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

        private List<UtvalgModification> _Modifications = new List<UtvalgModification>();
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

        private List<UtvalgKommune> _Kommuner = new List<UtvalgKommune>();
        public List<UtvalgKommune> Kommuner
        {
            get
            {
                return _Kommuner;
            }
            set
            {
                _Kommuner = value;
            }
        }



        private List<UtvalgDistrict> _Districts = new List<UtvalgDistrict>();
        public List<UtvalgDistrict> Districts
        {
            get
            {
                return _Districts;
            }
            set
            {
                _Districts = value;
            }
        }

        private List<UtvalgPostalZone> _PostalZones = new List<UtvalgPostalZone>();
        public List<UtvalgPostalZone> PostalZones
        {
            get
            {
                return _PostalZones;
            }
            set
            {
                _PostalZones = value;
            }
        }

        private UtvalgCriteriaList _Criterias = new UtvalgCriteriaList();
        public UtvalgCriteriaList Criterias
        {
            get
            {
                return _Criterias;
            }
            set
            {
                _Criterias = value;
            }
        }

        private List<CampaignDescription> _UtvalgsBasedOnMe;
        public List<CampaignDescription> UtvalgsBasedOnMe
        {
            get
            {
                return _UtvalgsBasedOnMe;
            }
            set
            {
                _UtvalgsBasedOnMe = value;
            }
        }
    }
}
