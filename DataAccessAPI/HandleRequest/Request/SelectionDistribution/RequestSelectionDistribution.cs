using Hangfire.Server;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.SelectionDistribution
{
    /// <summary>
    /// RequestSelectionDistribution
    /// </summary>
    public class RequestSelectionDistribution : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the utvalg identifier.
        /// </summary>
        /// <value>
        /// The utvalg identifier.
        /// </value>
        public string UtvalgId { get; set; }


        public PerformContext context { get; set; }

    }
}