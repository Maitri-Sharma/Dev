using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using Puma.Shared;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolerBySegmenterSearch
    /// </summary>
    public class RequestGetReolerBySegmenterSearch : IRequest<ResponseGetReolerBySegmenterSearch>
    {
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public DemographyOptions options { get; set; }
    }
}
