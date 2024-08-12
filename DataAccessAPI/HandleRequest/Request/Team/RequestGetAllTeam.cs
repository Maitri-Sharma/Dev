using DataAccessAPI.HandleRequest.Response.Team;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Team
{
    public class RequestGetAllTeam : IRequest<List<ResponseGetAllTeam>>
    {
    }
}
