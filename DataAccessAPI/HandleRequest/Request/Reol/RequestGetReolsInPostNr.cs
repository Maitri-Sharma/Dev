using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolsInPostNr
    /// </summary>
    public class RequestGetReolsInPostNr : IRequest<List<ResponseGetReolsInPostNr>>
    {
        /// <summary>
        /// Gets or sets the postnummer.
        /// </summary>
        /// <value>
        /// The postnummer.
        /// </value>
        public string postnummer { get; set; }
    }
}
