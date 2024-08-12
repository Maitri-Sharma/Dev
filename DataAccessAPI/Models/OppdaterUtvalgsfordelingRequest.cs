using System.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    public class OppdaterUtvalgsfordelingRequest
    {
        private Helper.UtvalgsIdListe UtvalgsIdListeField;

        public Helper.UtvalgsIdListe UtvalgsIdListe
        {
            get { return UtvalgsIdListeField; }
            set { UtvalgsIdListeField = value; }
        }

    }
}