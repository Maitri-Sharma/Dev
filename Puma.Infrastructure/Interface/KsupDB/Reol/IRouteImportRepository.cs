using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Shared;
using System.Data;
using System.Threading.Tasks;


namespace Puma.Infrastructure.Interface.KsupDB.Reol
{
    /// <summary>
    /// IRouteImportRepository
    /// </summary>
    public interface IRouteImportRepository : IKsupDBGenericRepository<utvalg>
    {
        /// <summary>
        /// Inputs the ruter delete all.
        /// </summary>
        /// <returns></returns>
        Task Input_Ruter_Delete_All();
        /// <summary>
        /// Inputs the ruter add one.
        /// </summary>
        /// <param name="ruteAntall">The rute antall.</param>
        /// <returns></returns>
        Task Input_Ruter_Add_One(Rute_Antall ruteAntall);
        /// <summary>
        /// Inputs the ruter kommune delete all.
        /// </summary>
        /// <returns></returns>
        Task Input_Ruter_Kommune_Delete_All();
        /// <summary>
        /// Inputs the ruter kommune add one.
        /// </summary>
        /// <param name="rute_AntallKommune">The rute antall kommune.</param>
        /// <returns></returns>
        Task Input_Ruter_Kommune_Add_One(Rute_AntallKommune rute_AntallKommune);
        /// <summary>
        /// Inputs the relopunkter delete all.
        /// </summary>
        /// <returns></returns>
        Task Input_Relopunkter_Delete_All();
        /// <summary>
        /// Inputs the relopunkter add one.
        /// </summary>
        /// <param name="rutePunkter">The rute punkter.</param>
        /// <returns></returns>
        Task Input_Relopunkter_Add_One(Rute_RutePunkter rutePunkter);
        /// <summary>
        /// Inputs the boksanlegg delete all.
        /// </summary>
        /// <returns></returns>
        Task Input_Boksanlegg_Delete_All();
        /// <summary>
        /// Inputs the boksanlegg add one.
        /// </summary>
        /// <param name="ruteBoksAnlegg">The rute boks anlegg.</param>
        /// <returns></returns>
        Task Input_Boksanlegg_Add_One(Rute_BoksAnlegg ruteBoksAnlegg);
    }
}
