using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.UtvalgList
{
    public class ResponseGetParentUtvalgListId
    {
        private int _parentutvalglistid;

        public int parentutvalglistid
        {
            get
            {
                return _parentutvalglistid;
            }
            set
            {
                _parentutvalglistid = value;
            }
        }
        
    }
}
