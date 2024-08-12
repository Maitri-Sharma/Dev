using Puma.DataLayer.DatabaseModel;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    /// <summary>
    /// IPostsoneReolMapperRepository
    /// </summary>
    public interface IPostsoneReolMapperRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        /// <summary>
        /// Gets the reol identifier postnummer mapping.
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetReolIdPostnummerMapping();
        /// <summary>
        /// Updates the postsone reol mapping table.
        /// </summary>
        /// <returns></returns>
        Task UpdatePostsoneReolMappingTable();
    }
}
