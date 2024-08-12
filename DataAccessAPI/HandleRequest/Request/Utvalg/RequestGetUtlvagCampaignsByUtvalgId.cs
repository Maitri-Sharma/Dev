using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestGetUtlvagCampaignsByUtvalgId
    /// </summary>
    public class RequestGetUtlvagCampaignsByUtvalgId : IRequest<List<ResponseGetUtlvagCampaignsByUtvalgId>>
    {
        /// <summary>
        /// Gets or sets the utvalg identifier.
        /// </summary>
        /// <value>
        /// The utvalg identifier.
        /// </value>
        public int UtvalgId { get; set; }
    }
}
