#region Namespaces
using DataAccessAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using MediatR;
using System.Net;
using DataAccessAPI.HandleRequest.Request.AddressPoints;
using DataAccessAPI.HandleRequest.Response.AddressPoints;
using DataAccessAPI.Extensions;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddresspointsStateController : ControllerBase
    {
        #region Variables
        private readonly ILogger<AddresspointsStateController> _logger;
        private readonly IMediator _mediator;

        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public AddresspointsStateController(ILogger<AddresspointsStateController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Save the address point. If exists then it iwll update otherwise adding new address point
        /// </summary>
        /// <param name="userId">Instance of user ID</param>
        /// <param name="addressPointList"> List of address point to be save</param>
        /// <returns>Number of row affecteds</returns>

        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("SaveAdressPointsAPI")]
        public IActionResult SaveAdressPointsAPI(string userId, AddressPointList addressPointList)
        {
            _logger.BeginScope("Inside into SaveAdressPointsAPI");
            #region Old Code
            //_logger.LogDebug("Inside into SaveAdressPointsAPI");

            //Exception exception = null;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];
            //int result = 0;

            //if (userId == null)
            //{
            //    exception = new Exception("userId can not be null for function SaveAdressPoints!");
            //    _logger.LogError(exception, exception.Message);
            //    throw exception;
            //}
            //if (addressPointList == null)
            //{
            //    exception = new Exception("addresspoints can not be null for function SaveAdressPoints!");
            //    _logger.LogError(exception, exception.Message);
            //    throw exception;
            //}

            //foreach (AddressPoint addressPoint in addressPointList)
            //{
            //    #region Parameter assignement

            //    npgsqlParameters[0] = new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = (addressPoint.Name.Length < 255 ? addressPoint.Name : addressPoint.Name.Substring(0, 255));

            //    npgsqlParameters[1] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[1].Value = userId;

            //    npgsqlParameters[2] = new NpgsqlParameter("p_x", NpgsqlTypes.NpgsqlDbType.Double);
            //    npgsqlParameters[2].Value = addressPoint.X;

            //    npgsqlParameters[3] = new NpgsqlParameter("p_y", NpgsqlTypes.NpgsqlDbType.Double);
            //    npgsqlParameters[3].Value = addressPoint.Y;

            //    npgsqlParameters[4] = new NpgsqlParameter("p_timecreated", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            //    npgsqlParameters[4].Value = DateTime.Now;

            //    npgsqlParameters[5] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[5].Direction = ParameterDirection.Output;


            //    #endregion

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //         dbhelper.ExecuteNonQuery("kspu_db.InertUpdateAddressPointsState", CommandType.StoredProcedure, npgsqlParameters);

            //        result = Convert.ToInt32(npgsqlParameters[5].Value);
            //        _logger.LogInformation(string.Format("Number of row affected {0} for Userid {1}", result, userId));
            //    }
            //}

            //_logger.LogDebug("Exiting from SaveAdressPointsAPI");

            //return result; 
            #endregion
            RequestSaveAddressPointsState request = new RequestSaveAddressPointsState()
            {
                userId = userId,
                addressPointList = addressPointList
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get the address points based on user id
        /// </summary>
        /// <param name="userId">User ID to fetch list of address related to passed user</param>
        /// <returns>List of Address points</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseAddressPointState>), (int)HttpStatusCode.OK)]
        [Route("GetAddressPointsState")]
        public IActionResult GetAddressPointsState(string userId)
        {
           
            #region Old COde
            //_logger.LogDebug("Inside into GetAddressPointsState");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //List<AddressPoint> result = new List<AddressPoint>();

            //npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = userId;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    dbhelper.ExecuteReader("kspu_db.getaddresspointsbyuserid", CommandType.StoredProcedure, (reader) => PopulateAddressPointList(reader, result), npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned {0}", result.Count);

            //_logger.LogDebug("Exiting from GetAddressPointsState");

            //return result; 
            #endregion
            RequestGetAddressPointsState request = new RequestGetAddressPointsState()
            {
                userId = userId
            };
            return Ok(_mediator.Send(request).Result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [Route("GenerateToken")]
        public IActionResult GenerateToken()
        {
            _logger.LogDebug("Inside into GenerateToken");
            RequesGetToken request = new RequesGetToken()
            { };
            return Ok(_mediator.Send(request).Result);
        }
        #endregion 

        #region private Methods

        /// <summary>
        /// Populates a list of AddressPoint from the dataReader.
        /// </summary>
        /// <param name="reader">An instance of IDataReader</param>
        /// <param name="addressPointsList">The list of Address points to populate.</param>
        /// <returns>True if found records, false otherwise.</returns>
        private static bool PopulateAddressPointList(IDataReader reader, List<AddressPoint> addressPointsList)
        {
            var recordsFound = false;
            while (reader.Read())
            {
                addressPointsList.Add(
                    new AddressPoint()
                    {
                        Name = reader.GetValueOrDefault<string>("r_name1"),
                        X = reader.GetValueOrDefault<double>("r_x"),
                        Y = reader.GetValueOrDefault<double>("r_y")
                    }
                );
                recordsFound = true;
            }
            return recordsFound;
        }

        #endregion


    }
}
