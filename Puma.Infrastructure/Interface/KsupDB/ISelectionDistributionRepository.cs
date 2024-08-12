using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    /// <summary>
    ///ISelectionDistributionRepository
    /// </summary>
    public interface ISelectionDistributionRepository : IKsupDBGenericRepository<selectionDistribution>
    {
        /// <summary>
        /// Gets the maximum identifier.
        /// </summary>
        /// <returns></returns>
        Task<int> GetMaxId();

        Task<List<selectionDistribution>> GetSelectionsForProcess();
    }
}
