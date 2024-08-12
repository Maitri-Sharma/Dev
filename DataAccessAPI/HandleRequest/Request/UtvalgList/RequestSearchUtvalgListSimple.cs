using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestSearchUtvalgListSimple
    /// </summary>
    public class RequestSearchUtvalgListSimple : IRequest<List<ResponseSearchUtvalgListSimple>>
    {
        /// <summary>
        /// Gets or sets the utvalglistname.
        /// </summary>
        /// <value>
        /// The utvalglistname.
        /// </value>
        public string utvalglistname { get; set; }
        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod searchMethod { get; set; }
    }
}
