using Npgsql;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface;
using Puma.Infrastructure.SQLDbHelper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Puma.Infrastructure.Repository.KspuDB;
using Puma.DataLayer.DatabaseModel;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System.Text;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class FylkeRepository : KsupDBGenericRepository<AddressPointsState>, IFylkeRepository
    {
        private readonly ILogger<FylkeRepository> _logger;
        public readonly string Connctionstring;
        public FylkeRepository(KspuDBContext context, ILogger<FylkeRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();

        }

        #region Public Methods

        /// <summary>
        /// To fetch Fylke data
        /// </summary>
        /// <returns>FylkeCollection data</returns>

        public async Task<FylkeCollection> GetAllFylkes()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAllFylkes");
            DataTable Fylker;
            FylkeCollection result = new FylkeCollection();

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                Fylker = dbhelper.FillDataTable("kspu_gdb.custom_getallfylkes", CommandType.StoredProcedure, null);
            }

            foreach (DataRow row in Fylker.Rows)
                result.Add(GetFylkeFromDataRow(row));


            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetAllFylkes");
            return result;
        }

        /// <summary>
        /// To fill data in Fylke object
        /// </summary>
        /// <param name="FylkeId"> Fylke ID to fetch data from database</param>
        /// <returns>Fylke data</returns>
        public async Task<Fylke> GetFylke(string FylkeId)
        {
            Exception exception = null;
            FylkeCollection fylkes = await GetAllFylkes();
            foreach (Fylke f in fylkes)
            {
                if (f.FylkeID == FylkeId)
                    return f;
            }
            exception = new Exception("Fant ikke fylket med fylkesid " + FylkeId + " i databasen.");
            throw exception;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To fill data in Fylke object
        /// </summary>
        /// <param name="row">Instance of Datarow</param>
        /// <returns>Fylke data</returns>
        private static Fylke GetFylkeFromDataRow(DataRow row)
        {
            Fylke f = new Fylke();
            f.FylkeID = row["r_fylke_id"].ToString();
            f.FylkeName = row["r_fylke"].ToString();
            return f;
        }

        #endregion
    }
}
