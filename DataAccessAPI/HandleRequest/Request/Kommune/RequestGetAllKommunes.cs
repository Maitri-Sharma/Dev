

using DataAccessAPI.HandleRequest.Response.Kommune;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Kommune
{
    public class RequestGetAllKommunes : IRequest<List<ResponseGetAllKommunes>>
    {
    }
}
