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
    /// RequestSearchUtvalgDetailById
    /// </summary>
    public class RequestSearchUtvalgDetailById : IRequest<List<ResponseGetUtlvagDetailById>>
    {
        /// <summary>
        /// Gets or sets the utlvag identifier.
        /// </summary>
        /// <value>
        /// The utlvag identifier.
        /// </value>
        public int UtlvagId { get; set; }

        /// <summary>
        /// Gets or sets the name of the current reol table.
        /// </summary>
        /// <value>
        /// The name of the current reol table.
        /// </value>
        [JsonIgnore]
        public string CurrentReolTableName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include reols].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include reols]; otherwise, <c>false</c>.
        /// </value>
        public bool includeReols { get; set; }

        /// <summary>
        /// customerNos
        /// </summary>
        public string customerNos { get; set; }
    }
}
