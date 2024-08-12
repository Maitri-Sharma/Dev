using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestSearchUtvalgByName
    /// </summary>
    public class RequestSearchUtvalgByName : IRequest<List<ResponseSearchUtvalgByName>>
    {
        /// <summary>
        /// Gets or sets the name of the utvalg.
        /// </summary>
        /// <value>
        /// The name of the utvalg.
        /// </value>
        public string UtvalgName { get; set; }

        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod SearchMethod { get; set; }
    }
}
