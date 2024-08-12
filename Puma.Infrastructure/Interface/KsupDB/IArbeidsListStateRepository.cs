using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
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
    /// IArbeidsListStateRepository
    /// </summary>
    public interface IArbeidsListStateRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        /// <summary>
        /// Saves the state of the arbeids list.
        /// </summary>
        /// <param name="arbeidsListState">State of the arbeids list.</param>
        /// <returns></returns>
        Task<int> SaveArbeidsListState(List<ArbeidsListEntryState> arbeidsListState);

        /// <summary>
        /// Gets the state of the arbeids list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task<List<ArbeidsListEntryState>> GetArbeidsListState(string userId);

        Task<bool> IsMaximumOneEntryActive(List<ArbeidsListEntryState> arbeidsListState);
    }
}
