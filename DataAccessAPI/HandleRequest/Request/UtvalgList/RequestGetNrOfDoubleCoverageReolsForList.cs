using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestGetNrOfDoubleCoverageReolsForList : IRequest<int>
    {
        public int listId { get; set; }
    }
}
