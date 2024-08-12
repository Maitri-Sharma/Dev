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
    /// SubtractRestkapasitetAbsoluttDagHandler
    /// </summary>
    public class SubtractRestkapasitetAbsoluttDagHandler : IRequestHandler<RequestSubtractRestkapasitetAbsoluttDag, bool>
    {
        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractRestkapasitetAbsoluttDagHandler"/> class.
        /// </summary>
        /// <param name="kapasitetRepository">The kapasitet repository.</param>
        /// <exception cref="System.ArgumentNullException">kapasitetRepository</exception>
        public SubtractRestkapasitetAbsoluttDagHandler(IKapasitetRepository kapasitetRepository)
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
        public async Task<bool> Handle(RequestSubtractRestkapasitetAbsoluttDag request, CancellationToken cancellationToken)
        {
            await _kapasitetRepository.SubtractRestkapasitetAbsoluttDag(request.ruteIds, request.restvekt, request.dato, request.mottakerType, request.restthickness);
            return true;
        }
    }
}
