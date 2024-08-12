using MediatR;
using Newtonsoft.Json;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestAcceptAllChangesOfList : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets List Id
        /// </summary>
        public int ListId { get; set; }


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
