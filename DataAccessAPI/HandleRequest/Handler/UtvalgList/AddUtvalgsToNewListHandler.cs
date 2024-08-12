using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// AddUtvalgsToNewListHandler
    /// </summary>
    public class AddUtvalgsToNewListHandler : IRequestHandler<RequestAddUtvalgsToNewList, ResponseAddUtvalgsToNewList>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<AddUtvalgsToNewListHandler> _logger;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddUtvalgsToNewListHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// utvalgRepository
        /// or
        /// mapper
        /// </exception>
        public AddUtvalgsToNewListHandler(IUtvalgListRepository utvalgListRepository, IUtvalgRepository utvalgRepository, ILogger<AddUtvalgsToNewListHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<ResponseAddUtvalgsToNewList> Handle(RequestAddUtvalgsToNewList request, CancellationToken cancellationToken)
        {
            ResponseAddUtvalgsToNewList response;

            Puma.Shared.UtvalgList newList = new Puma.Shared.UtvalgList();
            newList.Name = request.ListName;
            if (!string.IsNullOrEmpty(request.CustomerNo))
            {
                newList.KundeNummer = request.CustomerNo;
                newList.KundeNavn = request.CustomerName;
            }
            if (!string.IsNullOrEmpty(request.Logo)) newList.Logo = request.Logo;

            newList.IsBasis = request.utvalgs[0].IsBasis;
            if (newList.IsBasis)
            {
                newList.KundeNummer = request.utvalgs[0].KundeNummer;
                newList.KundeNavn = request.utvalgs[0].KundeNavn;
            }
            //Save utvalg list data
            _logger.LogDebug("Calling SaveUtvalgListData from Repository");
            var utvalgListId = await _utvalgListRepository.SaveUtvalgListData(newList, request.UserName);

            foreach (var itemUtvalg in request.utvalgs)
            {
                itemUtvalg.ListId = utvalgListId.ToString();
                itemUtvalg.ListName = newList.Name;
                if (string.IsNullOrEmpty(itemUtvalg.KundeNummer))
                {
                    itemUtvalg.KundeNummer = request.CustomerNo;
                    itemUtvalg.KundeNavn = request.CustomerName;
                }
                _logger.LogDebug("Calling SaveUtvalgData from Repository");
                _ = await _utvalgRepository.SaveUtvalgData(itemUtvalg, request.UserName, true);
            }
            _logger.LogDebug("Calling GetUtvalgListSimple from Repository");
            Puma.Shared.UtvalgList utvalgListData = await _utvalgListRepository.GetUtvalgListSimple(Convert.ToInt32(utvalgListId));

            if (utvalgListData != null)
            {
                utvalgListData.MemberUtvalgs = new List<Puma.Shared.Utvalg>();
                utvalgListData.MemberUtvalgs.AddRange(request.utvalgs);
            }
            response = _mapper.Map<Puma.Shared.UtvalgList, ResponseAddUtvalgsToNewList>(utvalgListData);
            utvalgListData.ModificationDate = DateTime.UtcNow;
            response.Modifications = new List<Puma.Shared.UtvalgModification>();
            response.Modifications.Add(new Puma.Shared.UtvalgModification()
            {
                ModificationTime = utvalgListData.ModificationDate
            });

            await GetListsToRefreshDueToUpdate(utvalgListData);

            return response;

        }

        /// <summary>
        /// Gets the lists to refresh due to update.
        /// </summary>
        /// <param name="utvalgList">The utvalg list.</param>
        private async Task GetListsToRefreshDueToUpdate(Puma.Shared.UtvalgList utvalgList)
        {
            List<long> ids = new List<long>();
            if (utvalgList.IsBasis)
            {
                _logger.LogDebug("Calling GetListsToRefreshDueToUpdateToBasisList from Repository");
                ids.AddRange(await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisList(utvalgList));


                _logger.LogDebug("Calling GetListsToRefreshDueToUpdateToBasisListChild from Repository");
                ids.AddRange(await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisListChild(utvalgList));

            }
            _logger.LogDebug("Calling SendBasisUtvalgFordelingToQue from Repository");
            await _utvalgListRepository.SendBasisUtvalgFordelingToQue(ids, "L");



        }
    }
}
