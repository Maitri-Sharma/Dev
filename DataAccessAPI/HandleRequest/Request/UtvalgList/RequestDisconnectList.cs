using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System.Text.Json.Serialization;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestDisconnectList
    /// </summary>
    public class RequestDisconnectList : IRequest<ResponseDisconnectList>
    {
        /// <summary>
        /// Gets or sets the utvalg list.
        /// </summary>
        /// <value>
        /// The utvalg list.
        /// </value>
        public int ListId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        
        [JsonIgnore]
        public string UserName{ get; set; }
    }
}
