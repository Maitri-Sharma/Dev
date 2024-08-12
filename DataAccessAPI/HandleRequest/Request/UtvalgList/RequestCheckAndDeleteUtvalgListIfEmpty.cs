using MediatR;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestCheckAndDeleteUtvalgListIfEmpty
    /// </summary>
    public class RequestCheckAndDeleteUtvalgListIfEmpty : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the utvalg list identifier.
        /// </summary>
        /// <value>
        /// The utvalg list identifier.
        /// </value>
        public int UtvalgListId { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string userName { get; set; }
    }
}
