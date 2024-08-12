using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace DataAccessAPI.ECPumaHelper
{
    public class UtvalgExtended: Puma.Shared.ECPumaData
    {
        private string _kundeNr;
        public object KundeNr
        {
            get
            {
                return _kundeNr;
            }
            set
            {
                this._kundeNr = (string)value;
            }
        }
    }
}
