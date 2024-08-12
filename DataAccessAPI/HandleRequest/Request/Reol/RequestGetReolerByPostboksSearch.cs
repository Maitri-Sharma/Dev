using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolerByPostboksSearch
    /// </summary>
    public class RequestGetReolerByPostboksSearch : IRequest<List<ResponseGetReolerByPostboksSearch>>
    {
        /// <summary>
        /// Gets or sets the postboks.
        /// </summary>
        /// <value>
        /// The postboks.
        /// </value>
        public string postboks { get; set; }
    }
}
