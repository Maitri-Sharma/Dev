using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestGetUtvalgListIdByUtvalgId
    /// </summary>
    public class RequestGetUtvalgListIdByUtvalgId : IRequest<int>
    {
        /// <summary>
        /// Gets or sets the utvalg identifier.
        /// </summary>
        /// <value>
        /// The utvalg identifier.
        /// </value>
        public int UtvalgId { get; set; }
    }
}
