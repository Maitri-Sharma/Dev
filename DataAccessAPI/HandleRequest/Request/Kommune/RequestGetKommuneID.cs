

using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Kommune
{
    /// <summary>
    /// RequestGetKommuneID
    /// </summary>
    public class RequestGetKommuneID : IRequest<string>
    {
        /// <summary>
        /// Gets or sets the kommunenavn.
        /// </summary>
        /// <value>
        /// The kommunenavn.
        /// </value>
        public string Kommunenavn { get; set; }
        /// <summary>
        /// Gets or sets the fylke navn.
        /// </summary>
        /// <value>
        /// The fylke navn.
        /// </value>
        public string FylkeNavn { get; set; }
    }
}
