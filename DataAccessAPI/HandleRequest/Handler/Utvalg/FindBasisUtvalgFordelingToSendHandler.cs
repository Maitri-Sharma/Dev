using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class FindBasisUtvalgFordelingToSendHandler : IRequestHandler<RequestFindBasisUtvalgFordelingToSend, List<ResponseUtvalgBasisFordeling>>
    {

        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<FindBasisUtvalgFordelingToSendHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public FindBasisUtvalgFordelingToSendHandler(IUtvalgRepository utvalgRepository, ILogger<FindBasisUtvalgFordelingToSendHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseUtvalgBasisFordeling>> Handle(RequestFindBasisUtvalgFordelingToSend request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling FindBasisUtvalgFordelingToSend from Repository");
            var utvalgData = await _utvalgRepository.FindBasisUtvalgFordelingToSend();
            List<ResponseUtvalgBasisFordeling> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.UtvalgBasisFordeling>, List<ResponseUtvalgBasisFordeling>>(utvalgData).ToList();
            }
            return response;

        }
    }
}
