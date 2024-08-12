

using DataAccessAPI.HandleRequest.Response.ReolerKommune;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.ReolerKommune
{
    /// <summary>
    /// RequestGetReolerKommune
    /// </summary>
    public class RequestGetReolerKommune : IRequest<ResponseGetReolerKommune>
    {
        /// <summary>
        /// Gets or sets the reol identifier.
        /// </summary>
        /// <value>
        /// The reol identifier.
        /// </value>
        public long ReolId { get; set; }
        /// <summary>
        /// Gets or sets the kommune identifier.
        /// </summary>
        /// <value>
        /// The kommune identifier.
        /// </value>
        public string KommuneId { get; set; }
    }
}
