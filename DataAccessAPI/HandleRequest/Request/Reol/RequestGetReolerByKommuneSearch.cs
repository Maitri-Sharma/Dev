using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolerByKommuneSearch
    /// </summary>
    public class RequestGetReolerByKommuneSearch : IRequest<List<ResponseGetReolerByKommuneSearch>>
    {
        /// <summary>
        /// Gets or sets the kummune ider.
        /// </summary>
        /// <value>
        /// The kummune ider.
        /// </value>
        public string kummuneIder { get; set; }
    }
}
