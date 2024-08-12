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
    public class CreateBasisUtvalgFordelingOppdateringHandler : IRequestHandler<RequestCreateBasisUtvalgFordelingOppdatering, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CreateBasisUtvalgFordelingOppdateringHandler> _logger;


        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public CreateBasisUtvalgFordelingOppdateringHandler(IUtvalgRepository utvalgRepository, ILogger<CreateBasisUtvalgFordelingOppdateringHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(RequestCreateBasisUtvalgFordelingOppdatering request, CancellationToken cancellationToken)
        {
            UtvalgBasisFordeling dataToUpdate = _mapper.Map<RequestCreateBasisUtvalgFordelingOppdatering, UtvalgBasisFordeling>(request);
            _logger.LogDebug("Calling CreateBasisUtvalgFordelingOppdatering from Repository");
            await _utvalgRepository.CreateBasisUtvalgFordelingOppdatering(dataToUpdate);
            return true;
        }
    }
}
