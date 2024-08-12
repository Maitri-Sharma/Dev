using MediatR;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Request.Utvalg;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestAddUtvalgsToExistingList
    /// </summary>
    public class RequestAddUtvalgsToExistingList : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the utvalg list.
        /// </summary>
        /// <value>
        /// The utvalg list.
        /// </value>
        public Puma.Shared.UtvalgList utvalgList { get; set; }

        /// <summary>
        /// Gets or sets the utvalgs.
        /// </summary>
        /// <value>
        /// The utvalgs.
        /// </value>
        public List<Puma.Shared.Utvalg> utvalgs { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [JsonIgnore]
        public string userName { get; set; }
    }
}
