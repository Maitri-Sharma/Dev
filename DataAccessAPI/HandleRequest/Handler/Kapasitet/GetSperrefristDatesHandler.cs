using DataAccessAPI.HandleRequest.Request.Kapasitet;
using MediatR;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Kapasitet
{
    /// <summary>
    /// GetSperrefristDatesHandler
    /// </summary>
    public class GetSperrefristDatesHandler : IRequestHandler<RequestGetSperrefristDates, List<DateTime>>
    {
        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSperrefristDatesHandler"/> class.
        /// </summary>
        /// <param name="kapasitetRepository">The kapasitet repository.</param>
        /// <exception cref="System.ArgumentNullException">kapasitetRepository</exception>
        public GetSperrefristDatesHandler(IKapasitetRepository kapasitetRepository)
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<List<DateTime>> Handle(RequestGetSperrefristDates request, CancellationToken cancellationToken)
        {
            return await _kapasitetRepository.GetSperrefristDates(request.dato, request.dayCount);
        }
    }
}
