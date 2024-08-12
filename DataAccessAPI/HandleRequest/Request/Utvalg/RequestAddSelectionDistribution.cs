using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    ///RequestSelectionDistribution
    /// </summary>
    public class RequestAddSelectionDistribution : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the selection identifier.
        /// </summary>
        /// <value>
        /// The selection identifier.
        /// </value>
        public long SelectionId { get; set; }
    }
}
