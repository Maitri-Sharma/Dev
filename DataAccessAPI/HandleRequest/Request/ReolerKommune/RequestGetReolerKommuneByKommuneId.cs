

using DataAccessAPI.HandleRequest.Response.ReolerKommune;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.ReolerKommune
{
    /// <summary>
    /// RequestGetReolerKommuneByKommuneId
    /// </summary>
    public class RequestGetReolerKommuneByKommuneId : IRequest<List<ResponseGetReolerKommuneByKommuneId>>
    {
        /// <summary>
        /// Gets or sets the kommune identifier.
        /// </summary>
        /// <value>
        /// The kommune identifier.
        /// </value>
        public string KommuneId { get; set; }
    }
}
