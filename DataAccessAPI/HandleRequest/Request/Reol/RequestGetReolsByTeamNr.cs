using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolsByTeamNr
    /// </summary>
    public class RequestGetReolsByTeamNr : IRequest<List<ResponseGetReolsByTeamNr>>
    {
        /// <summary>
        /// Gets or sets the team nr.
        /// </summary>
        /// <value>
        /// The team nr.
        /// </value>
        public string teamNr { get; set; }
    }
}
