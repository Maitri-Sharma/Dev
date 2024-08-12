using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestGetUtvalgReolIDs : IRequest<List<long>>
    {
        /// <summary>
        /// Gets or sets the utlvag identifier.
        /// </summary>
        /// <value>
        /// The utlvag identifier.
        /// </value>
        public int UtlvagId { get; set; }
    }
}
