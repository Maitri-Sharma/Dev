using MediatR;
using Newtonsoft.Json;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestSaveManyUtvalgs
    /// </summary>
    public class RequestSaveManyUtvalgs : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the utvalg list.
        /// </summary>
        /// <value>
        /// The utvalg list.
        /// </value>
        public Puma.Shared.UtvalgList UtvalgList { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [JsonIgnore]
        public string UserName { get; set; }
    }
}
