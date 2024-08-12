using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolsFromReolIDString
    /// </summary>
    public class RequestGetReolsFromReolIDString : IRequest<List<ResponseGetReolsFromReolIDString>>
    {
        /// <summary>
        /// Gets or sets the ids.
        /// </summary>
        /// <value>
        /// The ids.
        /// </value>
        public string ids { get; set; }
    }
}
