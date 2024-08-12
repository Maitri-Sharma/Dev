using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestGetUtlvagNameByUtvalgId
    /// </summary>
    public class RequestGetUtlvagNameByUtvalgId : IRequest<string>
    {
        /// <summary>
        /// Gets or sets the utvag identifier.
        /// </summary>
        /// <value>
        /// The utvag identifier.
        /// </value>
        public int UtvagId { get; set; }
    }
}
