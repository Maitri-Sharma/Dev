using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.UtvalgList
{
    public class ResponseGetUtvalgListReolIDs
    {
        private long _ReolId;

        public long ReolId
        {
            get
            {
                return _ReolId;
            }
            set
            {
                _ReolId = value;
            }
        }
    }
}
 