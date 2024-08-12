using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.EC_Data
{
    public class OrdreStatusRequestEntity
    {
        private string correlationID;

        public string CorrelationID
        {
            get { return correlationID; }
            set { correlationID = value; }
        }

        private OrdreStatusDataEntity ordreStatusData;

        public OrdreStatusDataEntity OrdreStatusData
        {
            get { return ordreStatusData; }
            set { ordreStatusData = value; }
        }
    }
}
