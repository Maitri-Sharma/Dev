#region Namespaces
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.ByDel;
using DataAccessAPI.HandleRequest.Response.ByDel;
using DataAccessAPI.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ByDelController : ControllerBase
    {
        #region Variables
        private readonly ILogger<ByDelController> _logger;
        private static BydelCollection _AllBydelsCache;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors             
        /// <summary>
        /// Initializes a new instance of the <see cref="ByDelController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// mediator
        /// or
        /// mapper
        /// </exception>
        public ByDelController(ILogger<ByDelController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// To fetch data from ByDEl Table
        /// </summary>
        /// <returns>BydelCollection data</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BydelCollection), (int)HttpStatusCode.OK)]
        [Route("GetAllBydels")]
        public IActionResult GetAllBydels()
        {
            _logger.BeginScope("Inside into GetAllBydels");
            BydelCollection result = new BydelCollection();

            if (_AllBydelsCache != null)
            {
                result = _AllBydelsCache;
            }
            else
            {
                var byDelData = _mediator.Send(new RequestGetAllBydels()).Result;
                var data = _mapper.Map<List<ResponseGetAllBydels>, List<Bydel>>(byDelData);
                result.AddRange(data);


                _AllBydelsCache = result;
            }

            return Ok(result);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To fill data in Bydel object
        /// </summary>
        /// <param name="row">Kommune ID to fetch data from Reoler Kommune table</param>
        /// <returns>BydelCollection data</returns>
        private static Bydel GetBydelFromDataRow(DataRow row)
        {
            Bydel bydel = new Bydel((row["r_bydel_id"].ToString()), (row["r_bydel"].ToString()), (row["r_komm"].ToString()));
            return bydel;
        }

        #endregion
    }
}
