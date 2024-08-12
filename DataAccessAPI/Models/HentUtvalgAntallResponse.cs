using System.ServiceModel;
using System.Collections.Generic;

namespace DataAccessAPI.Models
{
    [MessageContract]
    public class HentUtvalgAntallResponse : FeilHandtering
    {
        private Helper.UtvalgantallListe utvalgAntall;

        [MessageBodyMember(Order = 0)]
        public Helper.UtvalgantallListe UtvalgAntall
        {
            get { return utvalgAntall; }
            set { utvalgAntall = value; }
        }

    }
}


