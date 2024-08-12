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
    public class RequestGetUtvalgDetailByName : IRequest<List<ResponseGetUtlvagDetailByName>>
    {
        /// <summary>
        /// Gets or sets the name of the utvalg.
        /// </summary>
        /// <value>
        /// The name of the utvalg.
        /// </value>
        public string UtvalgName { get; set; }
       

        /// <summary>
        /// Gets or sets the current reol table bame.
        /// </summary>
        /// <value>
        /// The current reol table bame.
        /// </value>
        [JsonIgnore]
        public string currentReolTableBame { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [include reoler].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include reoler]; otherwise, <c>false</c>.
        /// </value>
        public bool includeReoler { get; set; }

        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod searchMethod { get; set; }
    }
}
