﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgBasisFordeling
    {

        private DateTime _Dato;
        public DateTime Dato
        {
            get { return _Dato; }
            set { _Dato = value; }
        }

        private long _ID; // UtvalgsID or UtvalgListID
        public long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _Utvalgtype; // U(tvalg) - L (iste)
        public string Utvalgtype
        {
            get { return _Utvalgtype; }
            set { _Utvalgtype = value; }
        }

        private bool _IsSendtOEBS;
        public bool IsSendtOEBS
        {
            get { return _IsSendtOEBS; }
            set { _IsSendtOEBS = value; }
        }

        private DateTime _DatoSendtOEBS;
        public DateTime DatoSendtOEBS
        {
            get { return _DatoSendtOEBS; }
            set { _DatoSendtOEBS = value; }
        }

    }
}
