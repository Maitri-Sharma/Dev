using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestIsUtvalgOrdered : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long ID { get; set; }
    }
}
