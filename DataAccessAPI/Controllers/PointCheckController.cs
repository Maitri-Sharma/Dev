// NOT REQUIRED ANYWHERE IN PUMA

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
    public class PointCheckController : ControllerBase
    {
        #region Variables
        private readonly ILogger<PointCheckController> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        public PointCheckController(ILogger<PointCheckController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// To get GAB Error
        /// </summary>
        /// <param name="adrnr">Kommune name to check in table</param>
        /// <returns>hits</returns>
        [HttpGet("GetGABError", Name = nameof(GetGABError))]
        public DataTable GetGABError(int adrnr)
        {
            _logger.BeginScope("Inside into GetGABError");
            DataTable hits;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_adrnr", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = adrnr;

            npgsqlParameters[1] = new NpgsqlParameter("p_adrnr1duplicate", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = adrnr;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                hits = dbhelper.FillDataTable("kspu_gdb.custom_getgaberror", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Number of row returned {0}", hits.Rows.Count);

            _logger.LogDebug("Exiting from GetGABError");
            return hits;
        }


        /// <summary>
        /// Get kommune no. from Postnummer table
        /// </summary>
        /// <param name="postNo">Post No.to fetch data from Postnummer table</param>
        /// <returns>Komm No.</returns>
        [HttpGet("GetKommNo", Name = nameof(GetKommNo))]
        public string GetKommNo(int postNo)
        {
            _logger.BeginScope("Inside into GetKommNo");
            DataTable kommNoTable;
            string kommNo = "";
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_postno", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = postNo;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                kommNoTable = dbhelper.FillDataTable("kspu_gdb.custom_getkommno", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (kommNoTable != null && kommNoTable.Rows.Count > 0)
            {
                if (kommNoTable.Rows[0]["r_komm_id"] != DBNull.Value)
                   kommNo = System.Convert.ToString(kommNoTable.Rows[0]["r_komm_id"]);
            }

            _logger.LogInformation("Number of row returned {0}", kommNo);

            _logger.LogDebug("Exiting from GetKommNo");
            return kommNo;
        }

        /// <summary>
        /// To check correct Municipality No.
        /// </summary>
        /// <param name="adrNo">adr no to check in table</param>
        /// <param name="kommId">Kommune ID to check in table</param>
        /// <returns>hits</returns>
        [HttpGet("HasGABCorrectMunicipalityNo", Name = nameof(HasGABCorrectMunicipalityNo))]
        public DataTable HasGABCorrectMunicipalityNo(int adrNo, int kommId)
        {
            _logger.BeginScope("Inside into HasGABCorrectMunicipalityNo");
            DataTable hits;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_adrno", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = adrNo;

            npgsqlParameters[1] = new NpgsqlParameter("p_kommid", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = kommId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                hits = dbhelper.FillDataTable("kspu_gdb.hasGABcorrectmunicipalityno", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Number of row returned {0}", hits.Rows.Count);

            _logger.LogDebug("Exiting from HasGABCorrectMunicipalityNo");
            return hits;
        }

        #endregion

    }
}
