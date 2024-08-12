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
    public class PostsoneReolMapperController : ControllerBase
    {

        #region Variables
        private readonly ILogger<PostsoneReolMapperController> _logger;
        private DataTable _reolIdPostnummerMapping = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        public PostsoneReolMapperController(ILogger<PostsoneReolMapperController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Properties

        private DataTable ReolIdPostnummerMapping
        {
            get
            {
                _logger.LogDebug("Inside into ReolIdPostnummerMapping");
                
                if (_reolIdPostnummerMapping != null)
                    return _reolIdPostnummerMapping;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    _reolIdPostnummerMapping = dbhelper.FillDataTable("kspu_gdb.reolidpostnummermapping", CommandType.StoredProcedure, null);
                }

                _logger.LogInformation("Number of row returned: ", _reolIdPostnummerMapping.Rows.Count);

                _logger.LogDebug("Exiting from ReolIdPostnummerMapping");

                return _reolIdPostnummerMapping;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///  Returns postal zone based in reolId
        ///  </summary>
        ///  <param name="reolId"></param>
        ///  <returns>postal zone</returns>
        ///  <remarks>input parameter is string because REOL_ID is represented as varchar in DB</remarks>
        [HttpGet("GetPostalZone", Name = nameof(GetPostalZone))]
        public string GetPostalZone(string reolId)
        {
            _logger.LogDebug("Inside into GetPostalZone");
            DataRow[] mappingRows = ReolIdPostnummerMapping.Select("r_reolid = '" + reolId + "'");
           
            if (mappingRows.Length == 1 && !mappingRows[0].IsNull("r_postnummer"))
                return Convert.ToString(mappingRows[0]["r_postnummer"]);
            
            return null;
        }

        /// <summary>
        ///  Update Posten Reol Mapping Table
        ///  </summary>
        [HttpPost("UpdatePostsoneReolMappingTable", Name = nameof(UpdatePostsoneReolMappingTable))]
        public void UpdatePostsoneReolMappingTable()
        {
            _logger.BeginScope("Inside into UpdatePostsoneReolMappingTable");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            int result;

            npgsqlParameters[0] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Direction = ParameterDirection.Output;
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                dbhelper.ExecuteNonQuery("kspu_gdb.updatepostsonereolmappingtable", CommandType.StoredProcedure, npgsqlParameters);
            }
            result = Convert.ToInt32(npgsqlParameters[1].Value);
            _logger.LogInformation(string.Format("Number of row affected {0} ", result));

            _logger.LogDebug("Exiting from UpdatePostsoneReolMappingTable");
        }
        #endregion

    }
}