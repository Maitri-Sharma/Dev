using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using Newtonsoft.Json;
using Puma.Shared;
using System.Collections.Generic;
namespace DataAccessAPI.HandleRequest.Request.Reol
{
    public class RequestGetReolerByIndexedDemographySearch : IRequest<ResponseGetReolerByDemographySearch>
    {
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public DemographyOptions options { get; set; }

        /// <summary>
        /// Gets or sets the utvalg.
        /// </summary>
        /// <value>
        /// The utvalg.
        /// </value>
        public Puma.Shared.Utvalg Utvalg { get; set; }

        
    }
}
