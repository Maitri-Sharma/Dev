using DataAccessAPI.HandleRequest.Request.ArbeidsListState;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace DataAccessAPI.HandleRequest.Handler.ArbeidsListState
{
    public class SaveArbeidsListStateHandler : IRequestHandler<RequestSaveArbeidsListState, int>
    {
        private readonly IArbeidsListStateRepository _arbeidsListStateRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SaveArbeidsListStateHandler> _logger;

        public SaveArbeidsListStateHandler(IArbeidsListStateRepository arbeidsListStateRepository, ILogger<SaveArbeidsListStateHandler> logger)
        {
            _arbeidsListStateRepository = arbeidsListStateRepository ?? throw new ArgumentNullException(nameof(arbeidsListStateRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(RequestSaveArbeidsListState request, CancellationToken cancellationToken)
        {
            Exception exception;
            if (request.arbeidsListState == null)
            {
                exception = new Exception("arbeidsListState can not be null!");
                _logger.LogError(exception, exception.Message);
                throw exception;
            }

            if (request.arbeidsListState?.Any() == true)
            {
                if (!await _arbeidsListStateRepository.IsMaximumOneEntryActive(request.arbeidsListState))
                {
                    exception = new Exception("More than one entry is active in arbeidsListState in sub SaveArbeidsListState");
                    _logger.LogError(exception, exception.Message);
                    throw exception;
                }

                return await _arbeidsListStateRepository.SaveArbeidsListState(request.arbeidsListState);
            }
            return 0;

        }
    }
}
