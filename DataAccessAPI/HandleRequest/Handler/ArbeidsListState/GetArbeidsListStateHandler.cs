using DataAccessAPI.HandleRequest.Request.ArbeidsListState;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DataAccessAPI.HandleRequest.Response.ArbeidsListState;
using System.Collections.Generic;
using AutoMapper;
using Puma.Shared;

namespace DataAccessAPI.HandleRequest.Handler.ArbeidsListState
{
    public class GetArbeidsListStateHandler : IRequestHandler<RequestGetArbeidsListState, List<ResponseArbeidsListEntryState>>
    {
        private readonly IArbeidsListStateRepository _arbeidsListStateRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetArbeidsListStateHandler> _logger;

        public IMapper _mapper;

        public GetArbeidsListStateHandler(IArbeidsListStateRepository arbeidsListStateRepository,
            ILogger<GetArbeidsListStateHandler> logger,
            IMapper mapper)
        {
            _arbeidsListStateRepository = arbeidsListStateRepository ?? throw new ArgumentNullException(nameof(arbeidsListStateRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public async Task<List<ResponseArbeidsListEntryState>> Handle(RequestGetArbeidsListState request, CancellationToken cancellationToken)
        {
            List<ResponseArbeidsListEntryState> responseArbeidsListEntryStates = null;
            _logger.LogDebug("Calling GetArbeidsListState from Repository");
            var arbeidsListData = await _arbeidsListStateRepository.GetArbeidsListState(request.userId);
            if (arbeidsListData?.Any() == true)
            {
                responseArbeidsListEntryStates = _mapper.Map<List<ArbeidsListEntryState>, List<ResponseArbeidsListEntryState>>(arbeidsListData);
            }
            return responseArbeidsListEntryStates;
        }
    }
}
