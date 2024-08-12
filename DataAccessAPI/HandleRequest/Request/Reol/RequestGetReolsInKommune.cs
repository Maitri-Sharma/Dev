using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    public class RequestGetReolsInKommune : IRequest<List<ResponseGetReolsInKommune>>
    {
        /// <summary>
        /// Gets or sets the kommune identifier.
        /// </summary>
        /// <value>
        /// The kommune identifier.
        /// </value>
        public string kommuneId { get; set; }
    }
}
