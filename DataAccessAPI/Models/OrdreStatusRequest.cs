using System.ServiceModel;
using System.Collections.Generic;

namespace DataAccessAPI.Models
{
    public class OrdreStatusRequest
    {
        private string correlationID;
        
        public string CorrelationID
        {
            get { return correlationID; }
            set { correlationID = value; }
        }

        private DataAccessAPI.Models.OrdreStatusData ordreStatusData;
        
        public DataAccessAPI.Models.OrdreStatusData OrdreStatusData
        {
            get { return ordreStatusData; }
            set { ordreStatusData = value; }
        }
    }
}