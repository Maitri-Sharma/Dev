using DataAccessAPI.HandleRequest.Response.Team;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Team
{
    public class RequestGetAllTeamKommuneKeys : IRequest<List<ResponseGetAllTeamKommuneKeys>>
    {
    }
}
