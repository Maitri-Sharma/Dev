using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;
namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolerByTeamSearch
    /// </summary>
    public class RequestGetReolerByTeamSearch : IRequest<List<ResponseGetReolerByTeamSearch>>
    {
        /// <summary>
        /// Gets or sets the team names.
        /// </summary>
        /// <value>
        /// The team names.
        /// </value>
        public string teamNames { get; set; }
    }
}
