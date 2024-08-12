using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSearchUtvalgListWithChildrenById : IRequest<List<ResponseSearchUtvalgListWithChildrenById>>
    {
        public int listId { get; set; }
        public bool includeReols { get; set; }
    }
}
