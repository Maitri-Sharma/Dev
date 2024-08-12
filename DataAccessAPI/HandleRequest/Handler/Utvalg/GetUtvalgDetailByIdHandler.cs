using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using DataAccessAPI.Hangfire;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class GetUtvalgDetailByIdHandler : IRequestHandler<RequestGetUtlvagDetailById, ResponseGetUtlvagDetailById>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgDetailByIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetUtvalgDetailByIdHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgDetailByIdHandler> logger,
             IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public async Task<ResponseGetUtlvagDetailById> Handle(RequestGetUtlvagDetailById request, CancellationToken cancellationToken)
        {
            //int a = 10;
           // BackgroundJob.Enqueue<IHangfireJob>(x => x.FireAndForgot(10));

            var utvalgData = await _utvalgRepository.GetUtvalgWithAllReferences(request.UtlvagId);

            ResponseGetUtlvagDetailById responseGet = null;
            if (utvalgData != null)
            {
                responseGet = _mapper.Map<Puma.Shared.Utvalg, ResponseGetUtlvagDetailById>(utvalgData);

            }

            return responseGet;
        }
    }
}
