using System.ServiceModel;
using System.Collections.Generic;

namespace DataAccessAPI.Models
{
    [MessageContract]
    public class UtvalgsfordelingMsg
    {
        private string correlationID;

        [MessageHeader(MustUnderstand = true)]
        public string CorrelationID
        {
            get { return correlationID; }
            set { correlationID = value; }
        }

        private DataAccessAPI.Models.Utvalgsfordeling utvalgsfordeling;

        [MessageBodyMember(Order = 0)]
        public DataAccessAPI.Models.Utvalgsfordeling Utvalgsfordeling
        {
            get { return utvalgsfordeling; }
            set { utvalgsfordeling = value; }
        }
    }
}


