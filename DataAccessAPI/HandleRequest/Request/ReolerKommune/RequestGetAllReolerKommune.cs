

using DataAccessAPI.HandleRequest.Response.ReolerKommune;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.ReolerKommune
{
    public class RequestGetAllReolerKommune : IRequest<List<ResponseGetAllReolerKommune>>
    {
    }
}
