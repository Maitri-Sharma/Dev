using DataAccessAPI.HandleRequest.Response.Team;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Team
{
    /// <summary>
    /// RequestSearchTeam
    /// </summary>
    public class RequestSearchTeam : IRequest<List<ResponseSearchTeam>>
    {
        /// <summary>
        /// Gets or sets the team navn.
        /// </summary>
        /// <value>
        /// The team navn.
        /// </value>
        public string teamNavn { get; set; }
    }
}
