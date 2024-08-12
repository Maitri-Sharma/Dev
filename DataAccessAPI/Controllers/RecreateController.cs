#region Namespaces
using DataAccessAPI.Extensions;
using DataAccessAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Data;
#endregion

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecreateController : ControllerBase
    {

        #region Variables
        private readonly ILogger<RecreateController> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        public RecreateController(ILogger<RecreateController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///  Create Table
        ///  </summary>
        /// <param name="tablename"></param>
        /// <param name="basedOn"></param>
        [HttpPost("CreateTable", Name = nameof(CreateTable))]
        public void CreateTable(string tablename, string basedOn = "")
        {
            _logger.BeginScope("Inside into CreateTable");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            npgsqlParameters = null;
            object result = null;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("CREATE TABLE " + tablename + " " + basedOn + " ", CommandType.Text, npgsqlParameters);
            }

            _logger.LogInformation("Result is {0}", result);

            _logger.LogDebug("Exiting from CreateTable");
        }

        /// <summary>
        ///  Drop Table
        ///  </summary>
        /// <param name="tablename"></param>
        [HttpPost("DropTable", Name = nameof(DropTable))]
        public bool DropTable(string tablename)
        {
            try
            {
                _logger.BeginScope("Inside into DropTable");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
                npgsqlParameters = null;
                object result = null;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery(" DROP TABLE " + tablename + " ", CommandType.Text, npgsqlParameters);
                }
                _logger.LogInformation("Is DropTable: {0}", result);

                _logger.LogDebug("Exiting from DropTable");
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("ORA-00942"))
                    // Logger.LogWarning(tableName & " does not exist")
                    return true;
                else
                {
                    _logger.LogError(exception.ToString());
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
