using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestGetParentUtvalgListId: IRequest<ResponseGetParentUtvalgListId>
    {
        public int listId { get; set; }
    }
}
