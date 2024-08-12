using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DataAccessAPI.Models
{
    [MessageContract]
    public class FinnUtvalgResponse : FeilHandtering
    {
        private Helper.UtvalgListe UtvalgField;

        [MessageBodyMember(Order = 0)]
        public Helper.UtvalgListe Utvalg
        {
            get { return UtvalgField; }
            set { UtvalgField = value; }
        }
    }
}
