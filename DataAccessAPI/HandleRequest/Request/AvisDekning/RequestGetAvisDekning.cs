using DataAccessAPI.HandleRequest.Response.AvisDekning;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.AvisDekning
{
    /// <summary>
    /// RequestGetAvisDekning
    /// </summary>
    public class RequestGetAvisDekning : IRequest<List<ResponseAvisDekning>>
    {
        /// <summary>
        /// Gets or sets the reol identifier.
        /// </summary>
        /// <value>
        /// The reol identifier.
        /// </value>
        public long reolId { get; set; }
    }
}
