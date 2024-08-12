using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using Newtonsoft.Json;
using Puma.Shared;
using System.Collections.Generic;
namespace DataAccessAPI.HandleRequest.Request.Reol
{
    /// <summary>
    /// RequestGetReolerByDemographySearch
    /// </summary>
    public class RequestGetReolerByDemographySearch : IRequest<ResponseGetReolerByDemographySearch>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is from kunde web.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is from kunde web; otherwise, <c>false</c>.
        /// </value>

        [JsonIgnore]
        public bool IsFromKundeWeb { get; set; }
    }
}
