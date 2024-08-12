using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolsInFylke
    /// </summary>
    public class RequestGetReolsInFylke : IRequest<List<ResponseGetReolsInFylke>>
    {
        /// <summary>
        /// Gets or sets the fylke identifier.
        /// </summary>
        /// <value>
        /// The fylke identifier.
        /// </value>
        public string fylkeId { get; set; }
    }
}
