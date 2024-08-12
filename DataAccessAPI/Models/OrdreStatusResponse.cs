using System.ServiceModel;
using System.Collections.Generic;

namespace DataAccessAPI.Models
{
    [MessageContract]
    public class OrdreStatusResponse
    {
        private System.String CorrelationIdField;

        [MessageBodyMember(Order = 0)]
        public System.String CorrelationId
        {
            get { return CorrelationIdField; }
            set { CorrelationIdField = value; }
        }

        private System.Boolean? WillRespondField;

        [MessageBodyMember(Order = 1)]
        public System.Boolean? WillRespond
        {
            get { return WillRespondField; }
            set { WillRespondField = value; }
        }

    }
}


