using DataAccessAPI.HandleRequest.Request.Kapasitet;
using MediatR;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Kapasitet
{
    /// <summary>
    /// SubtractRestkapasitetSperrefristHandler
    /// </summary>
    public class SubtractRestkapasitetSperrefristHandler : IRequestHandler<RequestSubtractRestkapasitetSperrefrist, bool>
    {
        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractRestkapasitetSperrefristHandler"/> class.
        /// </summary>
        /// <param name="kapasitetRepository">The kapasitet repository.</param>
        /// <exception cref="System.ArgumentNullException">kapasitetRepository</exception>
        public SubtractRestkapasitetSperrefristHandler(IKapasitetRepository kapasitetRepository)
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
        public async Task<bool> Handle(RequestSubtractRestkapasitetSperrefrist request, CancellationToken cancellationToken)
        {
            await _kapasitetRepository.SubtractRestkapasitetSperrefrist(request.ruteIds, request.restvekt, request.dato, request.mottakertype, request.restthickness);
            return true;
        }
    }
}
