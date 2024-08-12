using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Utvalg
{
    public class UtvalgEntity
    {







        //private bool _OldReolMapIsMissing = false;
        //public bool OldReolMapMissing
        //{
        //    get;set;
        //}



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


        //private bool _Changed = false;
        //public bool Changed
        //{
        //    get
        //    {
        //        return _Changed;
        //    }
        //    set
        //    {
        //        _Changed = value;
        //    }
        //}


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

        //public string ListName
        //{
        //    get; set;
        //}

        //private string _KundeNavn;

        //public string KundeNavn
        //{
        //    get
        //    {
        //        return _KundeNavn;
        //    }
        //    set
        //    {
        //        _KundeNavn = value;
        //    }
        //}

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
        private int _Skrivebeskyttet;
        public int Skrivebeskyttet
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

        ////private PumaEnum.DistributionType _DistributionType;
        public string DistributionType
        {
            get; set;
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

        private int _IsBasis;
        public int IsBasis
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

        ////private UtvalgList _List;

        ////public int utvalglistid { get; set; }
        public string ListId { get; set; }

        //private int _TotalAntall;

        public int TotalAntall
        {
            get; set;
        }

        ///// <summary>
        /////     ''' Referanse til en ordre eller et tilbud. Avhengig av gitt type
        /////     ''' </summary>
        /////     ''' <remarks></remarks>
        //private string _OrdreReferanse;
        public string OrdreReferanse
        {
            get;set;
        }

        /////// <summary>
        ///////     ''' Type Ordre. Tilbud eller Ordre
        ///////     ''' </summary>
        ///////     ''' <remarks></remarks>
        ////private PumaEnum.OrdreType _OrdreType;
        public string OrdreType
        {
            get;set;
        }

        ///// <summary>
        /////     ''' Status på en ordre eller tilbud
        /////     ''' </summary>
        /////     ''' <remarks></remarks>
        ////private PumaEnum.OrdreStatus _OrdreStatus;
        public string OrdreStatus
        {
            get;set;
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


        ///// <summary>
        /////     ''' Dato for når en ordre skal innleveres
        /////     ''' </summary>
        /////     ''' <remarks></remarks>
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

        ///// <summary>
        ///// Gets or sets the modification date.
        ///// </summary>
        ///// <value>
        ///// The modification date.
        ///// </value>
        public DateTime ModificationDate
        {
            get; set;
        }

        ///// <summary>
        /////     ''' Evt avtalenummer tilknyttet utvalg. Settes og benyttes av Ordre
        /////     ''' </summary>
        /////     ''' <remarks></remarks>
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




    }
}
