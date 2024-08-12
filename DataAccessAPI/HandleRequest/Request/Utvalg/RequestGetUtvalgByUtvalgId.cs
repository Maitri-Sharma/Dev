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
    /// RequestGetUtvalgByUtvalgId
    /// </summary>
    public class RequestGetUtvalgByUtvalgId : IRequest<ResponseGetUtlvagByUtvalgId>
    {
        /// <summary>
        /// Gets or sets the utlvag identifier.
        /// </summary>
        /// <value>
        /// The utlvag identifier.
        /// </value>
        public int utlvagId { get; set; }

        /// <summary>
        /// Gets or sets the name of the curretn reol table.
        /// </summary>
        /// <value>
        /// The name of the curretn reol table.
        /// </value>
        [JsonIgnore]
        public string CurretnReolTableName { get; set; }

        [JsonIgnore]
        public bool GetsummarizeData { get; set; } = false;
    }
}
