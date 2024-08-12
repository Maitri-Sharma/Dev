using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.KspuDB
{
    /// <summary>
    /// SelectionDistributionRepository
    /// </summary>
    public class SelectionDistributionRepository : KsupDBGenericRepository<selectionDistribution>, ISelectionDistributionRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SelectionDistributionRepository> _logger;
        /// <summary>
        /// The connctionstring
        /// </summary>
        public readonly string Connctionstring;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDistributionRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public SelectionDistributionRepository(KspuDBContext context, ILogger<SelectionDistributionRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        /// <summary>
        /// Gets the maximum identifier.
        /// </summary>
        /// <returns></returns>
        public Task<int> GetMaxId()
        {
            var data = _context.selectionDistributions.Select(x => x.Id);
            int max = 1;
            if (data?.Any() == true)
            {
                max = data.Max() + 1;
            }

            return Task.FromResult(max);
        }

        public async Task<List<selectionDistribution>> GetSelectionsForProcess()
        {
            return await (from selectionDist in _context.selectionDistributions
                         where selectionDist.IsOrderStatusUpdated == false
                         select selectionDist)?.ToListAsync();
        }
    }
}
