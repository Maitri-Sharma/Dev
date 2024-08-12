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
    public class ByDelRepository : KsupDBGenericRepository<AddressPointsState>, IByDelRepository
    {
        private readonly ILogger<ByDelRepository> _logger;
        public readonly string Connctionstring;

        public ByDelRepository(KspuDBContext context, ILogger<ByDelRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods
        /// <summary>
        /// To fetch data from ByDEl Table
        /// </summary>
        /// <returns>BydelCollection data</returns>
        public async Task<BydelCollection> GetAllBydels()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAllBydels");
            DataTable bydeler;
            BydelCollection result = new BydelCollection();
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                bydeler = dbhelper.FillDataTable("kspu_gdb.custom_getallbydels", CommandType.StoredProcedure, null);
            }
            foreach (DataRow row in bydeler.Rows)
            {
                result.Add(GetBydelFromDataRow(row));
            }

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetAllBydels");
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To fill data in Bydel object
        /// </summary>
        /// <param name="row">Kommune ID to fetch data from Reoler Kommune table</param>
        /// <returns>BydelCollection data</returns>
        private Bydel GetBydelFromDataRow(DataRow row)
        {
            Bydel bydel = new Bydel((row["r_bydel_id"].ToString()), (row["r_bydel"].ToString()), (row["r_komm"].ToString()));
            return bydel;
        }

        #endregion
    }
}
