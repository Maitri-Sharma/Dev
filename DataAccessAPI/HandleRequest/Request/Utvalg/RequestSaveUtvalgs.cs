using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestSaveUtvalgs
    /// </summary>
    public class RequestSaveUtvalgs : IRequest<List<ResponseSaveUtvalgs>>
    {
        /// <summary>
        /// Gets or sets the utvalgs.
        /// </summary>
        /// <value>
        /// The utvalgs.
        /// </value>
        public List<Puma.Shared.Utvalg> utvalgs { get; set; }

        [JsonIgnore]

        public string userName { get; set; }

        [JsonIgnore]
        public bool saveOldReoler { get; set; } = false;

        [JsonIgnore]
        public bool skipHistory { get; set; } = false;

        [JsonIgnore]
        public int forceUtvalgListId { get; set; } = 0;

    }
}
