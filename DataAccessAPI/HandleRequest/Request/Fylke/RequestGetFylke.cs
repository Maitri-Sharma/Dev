using DataAccessAPI.HandleRequest.Response.Fylke;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Fylke
{
    /// <summary>
    /// RequestGetFylke
    /// </summary>
    public class RequestGetFylke : IRequest<ResponseGetFylkes>
    {
        /// <summary>
        /// Gets or sets the fylke identifier.
        /// </summary>
        /// <value>
        /// The fylke identifier.
        /// </value>
        public string FylkeId { get; set; }
    }
}
