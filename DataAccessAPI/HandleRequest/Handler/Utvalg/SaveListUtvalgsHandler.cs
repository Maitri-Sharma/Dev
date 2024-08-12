using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class SaveListUtvalgsHandler : IRequestHandler<RequestListSaveUtvalgs, List<ResponseSaveUtvalg>>
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// utvalgRepository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SaveListUtvalgsHandler> _logger;

        /// <summary>
        /// Mapper
        /// </summary>
        private readonly IMapper _mapper;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <param name="utvalgRepository"></param>
        /// <param name="mapper"></param>

        public SaveListUtvalgsHandler(IMediator mediator, ILogger<SaveListUtvalgsHandler> logger, IUtvalgRepository utvalgRepository, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ResponseSaveUtvalg>> Handle(RequestListSaveUtvalgs request, CancellationToken cancellationToken)
        {

            List<ResponseSaveUtvalg> response = new();

            _logger.LogDebug("Request to save list of selection" + JsonConvert.SerializeObject(request));
            List<Puma.Shared.Utvalg> lstUtvalgs = new List<Puma.Shared.Utvalg>();
            if (request.utvalgs?.Any() == true)
            {
                foreach (var itemUtvalg in request.utvalgs)
                {
                    try
                    {
                        itemUtvalg.UtvalgId = await _utvalgRepository.GetSequenceNextVal("KSPU_DB.UtvalgId_Seq");
                        lstUtvalgs.Add(itemUtvalg);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error during save utvalg" + itemUtvalg.Name, ex);

                    }
                    //response.Add(responseSave);
                }

                //Once all data set call method to save all in DB at same time

                await _utvalgRepository.SaveMultipleUtvalgData(lstUtvalgs, request.userName);

               return response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseSaveUtvalg>>(lstUtvalgs);

                
            }
            return response;
        }
    }
}
