using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolFromReolID
    /// </summary>
    public class RequestGetReolFromReolID : IRequest<ResponseGetReolFromReolID>
    {
        /// <summary>
        /// Gets or sets the reol identifier.
        /// </summary>
        /// <value>
        /// The reol identifier.
        /// </value>
        public long reolID { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [get avis dekning].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [get avis dekning]; otherwise, <c>false</c>.
        /// </value>
        public bool getAvisDekning { get; set; } = true;
    }
}
