using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.AvisDekning
{
    public class ResponseAvis
    {
        private string _utgave;
        public string Utgave
        {
            get
            {
                return _utgave;
            }
            set
            {
                _utgave = value;
            }
        }

        private string _feltnavn;
        public string Feltnavn
        {
            get
            {
                return _feltnavn;
            }
            set
            {
                _feltnavn = value;
            }
        }
    }
}
