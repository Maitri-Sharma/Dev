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
    /// RequestListSaveUtvalgs
    /// </summary>
    public class RequestListSaveUtvalgs : IRequest<List<ResponseSaveUtvalg>>
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
      
    }
}
