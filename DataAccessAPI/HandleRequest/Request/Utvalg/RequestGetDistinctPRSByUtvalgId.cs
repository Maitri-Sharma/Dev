using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestGetDistinctPRSByUtvalgId : IRequest<List<string>>
    {
        /// <summary>
        /// Gets or sets the utvalgid.
        /// </summary>
        /// <value>
        /// The utvalgid.
        /// </value>
        public int Utvalgid { get; set; }

        /// <summary>
        /// Gets or sets the name of the current reol table.
        /// </summary>
        /// <value>
        /// The name of the current reol table.
        /// </value>
        [JsonIgnore]
        public string CurrentReolTableName { get; set; }
    }
}
