using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.AvisDekning
{
    public class ResponseAvisDekning
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

        private string _Utgave;
        public string Utgave
        {
            get
            {
                return _Utgave;
            }
            set
            {
                _Utgave = value;
            }
        }

        private double _Eksemplar;
        public double Eksemplar
        {
            get
            {
                return _Eksemplar;
            }
            set
            {
                _Eksemplar = value;
            }
        }

        private double _Prosent;
        public double Prosent
        {
            get
            {
                return _Prosent;
            }
            set
            {
                _Prosent = value;
            }
        }
    }
}
