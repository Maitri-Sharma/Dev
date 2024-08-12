using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Utvalg
{
    public class ResponseSearchUtvalgByName
    {
        private int _ReolCount = 0;
        public int ReolCount
        {
            get
            {
                return _ReolCount;
            }
            set
            {
                _ReolCount = value;
            }
        }

        private string _UtvalgName;
        public string UtvalgName
        {
            get
            {
                return _UtvalgName;
            }
            set
            {
                _UtvalgName = value;
            }
        }


        private long _UtvalgId;

        public long UtvalgId
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

        private Puma.Shared.UtvalgList _List;

        public Puma.Shared.UtvalgList List
        {
            get
            {
                return _List;
            }
            set
            {
                _List = value;
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

        private PumaEnum.OrdreType _OrdreType;
        public PumaEnum.OrdreType OrdreType
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
    }
}
