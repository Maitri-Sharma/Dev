using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolsInTeam
    /// </summary>
    public class RequestGetReolsInTeam : IRequest<List<ResponseGetReolsInTeam>>
    {
        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        /// <value>
        /// The name of the team.
        /// </value>
        public List<string> teamName { get; set; }
    }
}
