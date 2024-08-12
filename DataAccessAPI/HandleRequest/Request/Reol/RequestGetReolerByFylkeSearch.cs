using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolerByFylkeSearch
    /// </summary>
    public class RequestGetReolerByFylkeSearch : IRequest<List<ResponseGetReolerByFylkeSearch>>
    {
        /// <summary>
        /// Gets or sets the fylke ider.
        /// </summary>
        /// <value>
        /// The fylke ider.
        /// </value>
        public string fylkeIder { get; set; }
    }
}
