using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.ArbeidsListState
{
    public class ResponseArbeidsListEntryState
    {
        public int Id { get; set; }


        public PumaEnum.ListType Type { get; set; }

        public string UserId{ get; set; }

        public bool Active { get; set; }
    }
}
