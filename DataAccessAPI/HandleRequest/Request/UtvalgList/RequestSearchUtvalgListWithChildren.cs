using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestSearchUtvalgListWithChildren
    /// </summary>
    public class RequestSearchUtvalgListWithChildren : IRequest<List<ResponseSearchUtvalgListWithChildren>>
    {
        /// <summary>
        /// Gets or sets the utvalglistname.
        /// </summary>
        /// <value>
        /// The utvalglistname.
        /// </value>
        public string Utvalglistname { get; set; }
        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod searchMethod { get; set; }
    }
}
