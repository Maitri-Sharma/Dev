using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestGetReolerByPostalZoneSearch : IRequest<List<ResponseGetReolerByPostalZoneSearch>>
    {
        /// <summary>
        /// Gets or sets the postal zone.
        /// </summary>
        /// <value>
        /// The postal zone.
        /// </value>
        public string postalZone { get; set; }
    }
}
