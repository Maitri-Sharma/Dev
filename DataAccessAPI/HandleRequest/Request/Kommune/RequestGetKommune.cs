

using DataAccessAPI.HandleRequest.Response.Kommune;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Kommune
{
    /// <summary>
    /// RequestGetKommune
    /// </summary>
    public class RequestGetKommune : IRequest<ResponseGetKommune>
    {
        /// <summary>
        /// Gets or sets the kommune identifier.
        /// </summary>
        /// <value>
        /// The kommune identifier.
        /// </value>
        public string KommuneId { get; set; }
    }
}
