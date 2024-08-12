using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolsFromReolIDs
    /// </summary>
    public class RequestGetReolsFromReolIDs : IRequest<List<ResponseGetReolsFromReolIDString>>
    {
        /// <summary>
        /// Gets or sets the reol i ds.
        /// </summary>
        /// <value>
        /// The reol i ds.
        /// </value>
        public long[] ReolIDs { get; set; }
    }
}
