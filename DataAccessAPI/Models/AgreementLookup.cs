using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.Models
{
    public class AgreementLookup
    {
        public HeaderFine Header { get; set; }
        public string BrukerNavn { get; set; }
        public string Key { get; set; }
        public string UtvalgsID { get; set; }
    }

    public class AvtaleOppslagResponse
    {
        public string KundeNr { get; set; }
        public decimal? statuskode { get; set; }
        public string AvtaleData { get; set; }
    }

    public class AgreementLookupResponse
    {
        public AvtaleOppslagResponse AvtaleOppslagResponse { get; set; }
    }
}
