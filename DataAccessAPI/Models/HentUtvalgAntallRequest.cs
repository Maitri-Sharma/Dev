using System.ServiceModel;
using System.Collections.Generic;

namespace DataAccessAPI.Models
{
    public class HentUtvalgAntallRequest
    {
        private DataAccessAPI.Models.UtvalgsId UtvalgsIdField;

        public DataAccessAPI.Models.UtvalgsId UtvalgsId
        {
            get { return UtvalgsIdField; }
            set { UtvalgsIdField = value; }
        }
    }
}


