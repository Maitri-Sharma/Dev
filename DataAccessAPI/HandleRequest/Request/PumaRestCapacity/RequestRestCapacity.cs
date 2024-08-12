using Hangfire.Server;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.PumaRestCapacity
{
    /// <summary>
    /// RequestSelectionDistribution
    /// </summary>
    public class RequestRestCapacity : IRequest<bool>
    {
        public PerformContext context { get; set; }

    }
}