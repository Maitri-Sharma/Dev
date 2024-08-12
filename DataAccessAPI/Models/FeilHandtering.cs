using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataAccessAPI.Models
{
    [MessageContract]
    public abstract class FeilHandtering
    {
        private string feilBeskrivelse;

        private int feilKode;

        [MessageBodyMember(Order = 3)]
        public string Feilbeskrivelse
        {
            get { return feilBeskrivelse; }
            set { feilBeskrivelse = value; }
        }

        [MessageBodyMember(Order = 2)]
        public int Feilkode
        {
            get { return feilKode; }
            set { feilKode = value; }
        }
    }
}
