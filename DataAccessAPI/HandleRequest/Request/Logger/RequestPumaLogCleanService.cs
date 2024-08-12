using Hangfire.Server;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Logger
{
    /// <summary>
    /// Request class for puma log clean service
    /// </summary>
    public class RequestPumaLogCleanService : IRequest<bool>
    {
        /// <summary>
        /// Peform context hangfire
        /// </summary>
        public PerformContext context { get; set; }
    }
}
