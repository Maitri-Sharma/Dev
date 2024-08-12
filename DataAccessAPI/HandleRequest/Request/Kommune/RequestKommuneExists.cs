

using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Kommune
{
    /// <summary>
    ///RequestKommuneExists
    /// </summary>
    public class RequestKommuneExists : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the kommunenavn.
        /// </summary>
        /// <value>
        /// The kommunenavn.
        /// </value>
        public string Kommunenavn { get; set; }
    }
}
