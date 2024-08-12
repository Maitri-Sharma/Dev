#region Namespaces
using DataAccessAPI.Extensions;
using DataAccessAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Puma.Shared;
using System;
using System.Data;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostnrController : ControllerBase
    {

        #region Variables
        private readonly ILogger<PostnrController> _logger;
        private static PostnrCollection _allPostNumbers = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        public PostnrController(ILogger<PostnrController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Properties
        private PostnrCollection AllPostNumbers
        {
            get
            {
                _logger.LogDebug("Inside into AllPostNumbers");
                if (_allPostNumbers != null)
                    return _allPostNumbers;
                DataTable dataTable;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    dataTable = dbhelper.FillDataTable("kspu_gdb.custom_allpostnumbers", CommandType.StoredProcedure, null);
                }
                _allPostNumbers = CreatePostnummerCollection(dataTable);
                _logger.LogInformation("Number of row returned: ", _allPostNumbers.Count);

                _logger.LogDebug("Exiting from AllPostNumbers");

                return _allPostNumbers;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// To fetch Post number data
        /// </summary>
        /// <returns>PostnrCollection</returns>
        [HttpGet("GetAllPostNr", Name = nameof(GetAllPostNr))]
        public PostnrCollection GetAllPostNr()
        {
            return AllPostNumbers;
        }

        /// <summary>
        /// To fetch data from Post nummer table
        /// </summary>
        /// <param name="postnr">Post no. to fetch the data</param>
        /// <returns>PostnrCollection</returns>
        [HttpGet("GetPostNr", Name = nameof(GetPostNr))]
        public PostnrCollection GetPostNr(string postnr)
        {
            _logger.BeginScope("Inside into GetPostNr");
            PostnrCollection result = new PostnrCollection();

            foreach (Postnr postnummer in AllPostNumbers)
            {
                if (postnummer?.Post_nr == InsertBeginningZeros(postnr))
                {
                    result.Add(postnummer);
                    break;
                }
            }
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetPostNr");
            return result;
        }

        /// <summary>
        /// To fetch data from Post nummer table
        /// </summary>
        /// <param name="PostnrFrom">Post no. From to fetch the data</param>
        /// <param name="PostnrTo">Post no. To to fetch the data</param>
        /// <returns>PostnrCollection</returns>
        [HttpGet("GetPostNrFromTo", Name = nameof(GetPostNrFromTo))]
        public PostnrCollection GetPostNrFromTo(string PostnrFrom, string PostnrTo)
        {
            _logger.LogDebug("Inside into GetPostNrFromTo");
            PostnrCollection result = new PostnrCollection();

            int fromPostnrInteger = int.Parse(PostnrFrom);
            int toPostnnrInteger = int.Parse(PostnrTo);
            int comparePostnummerInteger;

            foreach (Postnr postnummer in AllPostNumbers)
            {
                comparePostnummerInteger = int.Parse(postnummer.Post_nr);
                if (comparePostnummerInteger >= fromPostnrInteger & comparePostnummerInteger <= toPostnnrInteger)
                    result.Add(postnummer);
            }

            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetPostNrFromTo");
            return result;
        }

        #endregion;

        #region Private Methods

        /// <summary>
        /// To create Post number collection data
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>PostnrCollection</returns>
        private PostnrCollection CreatePostnummerCollection(DataTable dataTable)
        {
            _logger.LogDebug("Inside into CreatePostnummerCollection");
            PostnrCollection result = new PostnrCollection();

            foreach (DataRow datarow in dataTable?.Rows)
            {
                if (!datarow.IsNull("r_postnr"))
                    result.Add(new Postnr() { Post_nr = Convert.ToString(datarow["r_postnr"]), Poststed = Convert.ToString(datarow["r_poststed"]) });
            }
            
            _logger.LogDebug("Existing from CreatePostnummerCollection");
            
            return result;
        }

        /// <summary>
        /// Insert extra zeoes in beginning
        /// </summary>
        /// <param name="postNr"></param>
        /// <returns>Updated Postnr</returns>
        private string InsertBeginningZeros(string postNr)
        {
            _logger.LogDebug("Inside into InsertBeginningZeros");
            string result = string.Empty;

            if (postNr?.Length >= 4)
                result = postNr;
            else
            {
                for (int i = 0; i <= 4 - postNr.Length - 1; i++)
                    result += "0";

                result += postNr;
            }

            _logger.LogDebug("Exiting from InsertBeginningZeros");
            return result;
        }
        #endregion
    }
}