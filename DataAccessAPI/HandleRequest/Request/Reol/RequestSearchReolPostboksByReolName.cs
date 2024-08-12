using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestSearchReolPostboksByReolName
    /// </summary>
    public class RequestSearchReolPostboksByReolName : IRequest<List<ResponseSearchReolPostboksByReolName>>
    {
        /// <summary>
        /// Gets or sets the name of the reol.
        /// </summary>
        /// <value>
        /// The name of the reol.
        /// </value>
        public string ReolName { get; set; }
        /// <summary>
        /// Gets or sets the name of the kommune.
        /// </summary>
        /// <value>
        /// The name of the kommune.
        /// </value>
        public string KommuneName { get; set; }
    }
}
