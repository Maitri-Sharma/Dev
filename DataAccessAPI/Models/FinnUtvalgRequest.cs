using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataAccessAPI.Models
{
    public class FinnUtvalgRequest
    {
        private FinnUtvalgData finnUtvalg;

        public FinnUtvalgData FinnUtvalg
        {
            get { return finnUtvalg; }
            set { finnUtvalg = value; }
        }

    }
}
