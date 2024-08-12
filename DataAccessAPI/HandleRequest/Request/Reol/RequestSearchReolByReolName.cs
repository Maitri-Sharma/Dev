using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;


namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    ///RequestSearchReolByReolName
    /// </summary>
    public class RequestSearchReolByReolName : IRequest<List<ResponseSearchReolByReolName>>
    {
        /// <summary>
        /// Gets or sets the name of the reol.
        /// </summary>
        /// <value>
        /// The name of the reol.
        /// </value>
        public string reolName { get; set; }
    }
}
