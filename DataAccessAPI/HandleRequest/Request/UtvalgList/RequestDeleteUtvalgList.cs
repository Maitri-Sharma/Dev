using MediatR;


namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestDeleteUtvalgList
    /// </summary>
    public class RequestDeleteUtvalgList : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the utvalg list identifier.
        /// </summary>
        /// <value>
        /// The utvalg list identifier.
        /// </value>
        public int UtvalgListId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [with children].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [with children]; otherwise, <c>false</c>.
        /// </value>
        public bool withChildren { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string userName { get; set; }
    }
}
