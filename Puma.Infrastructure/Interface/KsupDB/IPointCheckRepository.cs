using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using System.Data;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    /// <summary>
    /// IPointCheckRepository
    /// </summary>
    public interface IPointCheckRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        /// <summary>
        /// Gets the gab error.
        /// </summary>
        /// <param name="adrnr">The adrnr.</param>
        /// <returns></returns>
        Task<DataTable> GetGABError(int adrnr);
        /// <summary>
        /// Gets the komm no.
        /// </summary>
        /// <param name="postNo">The post no.</param>
        /// <returns></returns>
        Task<string> GetKommNo(int postNo);

        /// <summary>
        /// Determines whether [has gab correct municipality no] [the specified adr no].
        /// </summary>
        /// <param name="adrNo">The adr no.</param>
        /// <param name="kommId">The komm identifier.</param>
        /// <returns></returns>
        Task<DataTable> HasGABCorrectMunicipalityNo(int adrNo, int kommId);
    }
}
