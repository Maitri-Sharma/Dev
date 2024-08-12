﻿using AutoMapper;
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
    public class SearchUtvalgByUtvalListIdHandler : IRequestHandler<RequestSearchUtvalgByUtvalListId, List<ResponseSearchUtvalgByUtvalListId>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByUtvalListIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgByUtvalListIdHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByUtvalListIdHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseSearchUtvalgByUtvalListId>> Handle(RequestSearchUtvalgByUtvalListId request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgByUtvalListId from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalgByUtvalListId(request.UtlvagId,request.IncludeReols);
            List<ResponseSearchUtvalgByUtvalListId> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseSearchUtvalgByUtvalListId>>(utvalgData).ToList();
            }
            return response;
        }
    }
}