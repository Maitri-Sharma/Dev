

using DataAccessAPI.HandleRequest.Request.Kapasitet;
using MediatR;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Kapasitet
{
    /// <summary>
    /// GetTotalAntallHandler
    /// </summary>
    public class GetTotalAntallHandler : IRequestHandler<RequestGetTotalAntall, long>
    {
        /// <summary>
        /// The kapasitet repository/
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTotalAntallHandler"/> class.
        /// </summary>
        /// <param name="kapasitetRepository">The kapasitet repository.</param>
        /// <exception cref="System.ArgumentNullException">kapasitetRepository</exception>
        public GetTotalAntallHandler(IKapasitetRepository kapasitetRepository)
        {
            _kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<long> Handle(RequestGetTotalAntall request, CancellationToken cancellationToken)
        {
            return await _kapasitetRepository.GetTotalAntall(request.id, request.type);
        }
    }
}
