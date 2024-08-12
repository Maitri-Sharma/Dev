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
    public class BasisUtvalgFordelingExistsOnQueHandler : IRequestHandler<RequestBasisUtvalgFordelingExistsOnQue, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<BasisUtvalgFordelingExistsOnQueHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public BasisUtvalgFordelingExistsOnQueHandler(IUtvalgRepository utvalgRepository, ILogger<BasisUtvalgFordelingExistsOnQueHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(RequestBasisUtvalgFordelingExistsOnQue request, CancellationToken cancellationToken)
        {
            UtvalgBasisFordeling dataToUpdate = _mapper.Map<RequestBasisUtvalgFordelingExistsOnQue, UtvalgBasisFordeling>(request);
            _logger.LogDebug("Calling  BasisUtvalgFordelingExistsOnQue from Repository");
            return await _utvalgRepository.BasisUtvalgFordelingExistsOnQue(dataToUpdate);
        }
    }
}
