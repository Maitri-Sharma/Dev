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
    /// RequestGetUtlvagDetailById
    /// </summary>
    public class RequestGetUtlvagDetailById : IRequest<ResponseGetUtlvagDetailById>
    {
        /// <summary>
        /// Gets or sets the utlvag identifier.
        /// </summary>
        /// <value>
        /// The utlvag identifier.
        /// </value>
        public int UtlvagId { get; set; }

        [JsonIgnore]
        public string CurrentReolTableName { get; set; }
    }
}
