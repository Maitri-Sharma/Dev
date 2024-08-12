using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestSearchUtvalgByOrdreReferanse : IRequest<List<ResponseSearchUtvalgByOrdreReferanse>>
    {
        /// <summary>
        /// Gets or sets the ordre referanse.
        /// </summary>
        /// <value>
        /// The ordre referanse.
        /// </value>
        public string OrdreReferanse { get; set; }

        /// <summary>
        /// Gets or sets the type of the ordre.
        /// </summary>
        /// <value>
        /// The type of the ordre.
        /// </value>
        public string OrdreType { get; set; }


        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod SearchMethod { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [include reols].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include reols]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeReols { get; set; }

        /// <summary>
        /// Gets or sets the current reol table.
        /// </summary>
        /// <value>
        /// The current reol table.
        /// </value>
        [JsonIgnore]
        public string CurrentReolTableName { get; set; }
    }
}
