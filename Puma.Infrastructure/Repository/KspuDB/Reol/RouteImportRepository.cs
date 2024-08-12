using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Shared;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.KspuDB.Reol
{
    /// <summary>
    /// RouteImportRepository
    /// </summary>
    public class RouteImportRepository : KsupDBGenericRepository<utvalg>, IRouteImportRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<RouteImportRepository> _logger;
        /// <summary>
        /// The connectionstring
        /// </summary>
        public readonly string connectionstring;
        //private string _DBConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteImportRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public RouteImportRepository(KspuDBContext context, ILogger<RouteImportRepository> logger) : base(context)
        {
            _logger = logger;
            connectionstring = _context.Database.GetConnectionString();
        }

        #region Route_Antall
        /// <summary>
        /// Inputs the ruter delete all.
        /// </summary>
        public async Task Input_Ruter_Delete_All()
        {

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            int result;
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for Input_Ruter_Delete_All");
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleterouteantalldata", CommandType.StoredProcedure, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Input_Ruter_Delete_All:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from Input_Ruter_Delete_All");
        }

        /// <summary>
        /// Inputs the ruter add one.
        /// </summary>
        /// <param name="ruteAntall">The rute antall.</param>
        public async Task Input_Ruter_Add_One(Rute_Antall ruteAntall)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[42];
            int result;
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for Input_Ruter_Add_One");
                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("reol_id_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = ruteAntall.ReolID;

                npgsqlParameters[1] = new NpgsqlParameter("reolnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = ruteAntall.ReolNr;

                npgsqlParameters[2] = new NpgsqlParameter("reolnavn_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = DBNull.Value;

                npgsqlParameters[3] = new NpgsqlParameter("reoltype_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[3].Value = ruteAntall.ReolType;

                npgsqlParameters[4] = new NpgsqlParameter("teamnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[4].Value = ruteAntall.TeamNr;

                npgsqlParameters[5] = new NpgsqlParameter("teamnavn_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[5].Value = ruteAntall.TeamNavn;

                npgsqlParameters[6] = new NpgsqlParameter("prsnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[6].Value = ruteAntall.PrsNr;

                npgsqlParameters[7] = new NpgsqlParameter("prsnavn_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[7].Value = ruteAntall.PrsNavn;

                npgsqlParameters[8] = new NpgsqlParameter("prsbeskrivelse_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[8].Value = ruteAntall.PrsBeskrivelse;

                npgsqlParameters[9] = new NpgsqlParameter("beskrivelse_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[9].Value = ruteAntall.Beskrivelse;

                npgsqlParameters[10] = new NpgsqlParameter("kommentar_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[10].Value = DBNull.Value;

                npgsqlParameters[11] = new NpgsqlParameter("pbkontnavn_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[11].Value = ruteAntall.PbKontNavn;

                npgsqlParameters[12] = new NpgsqlParameter("postnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[12].Value = ruteAntall.PostNr;

                npgsqlParameters[13] = new NpgsqlParameter("poststed_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[13].Value = ruteAntall.PostAdresse;

                npgsqlParameters[14] = new NpgsqlParameter("kommuneid_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[14].Value = ruteAntall.KommuneNr;

                npgsqlParameters[15] = new NpgsqlParameter("kommune_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[15].Value = ruteAntall.KommuneNavn;

                npgsqlParameters[16] = new NpgsqlParameter("fylkeid_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[16].Value = ruteAntall.FylkesNr;

                npgsqlParameters[17] = new NpgsqlParameter("fylke_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[17].Value = ruteAntall.FylkesNavn;

                npgsqlParameters[18] = new NpgsqlParameter("hh_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[18].Value = ruteAntall.Antall_HH_U_RES;

                npgsqlParameters[19] = new NpgsqlParameter("er_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[19].Value = 0;

                npgsqlParameters[20] = new NpgsqlParameter("gb_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[20].Value = 0;

                npgsqlParameters[21] = new NpgsqlParameter("vh_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[21].Value = ruteAntall.Antall_VH_U_RES;

                npgsqlParameters[22] = new NpgsqlParameter("hh_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[22].Value = ruteAntall.Antall_HH_M_RES;

                npgsqlParameters[23] = new NpgsqlParameter("er_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[23].Value = 0;

                npgsqlParameters[24] = new NpgsqlParameter("gb_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[24].Value = 0;

                npgsqlParameters[25] = new NpgsqlParameter("prissone_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[25].Value = ruteAntall.PrisSone;

                npgsqlParameters[26] = new NpgsqlParameter("regnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[26].Value = ruteAntall.ReolNr;

                npgsqlParameters[27] = new NpgsqlParameter("vh_m_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[27].Value = ruteAntall.Antall_VH_M_RES;

                npgsqlParameters[28] = new NpgsqlParameter("hh_gr_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[28].Value = ruteAntall.Antall_HH_GR_RES;

                npgsqlParameters[29] = new NpgsqlParameter("l_u_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[29].Value = ruteAntall.Antall_L_U_RES;

                npgsqlParameters[30] = new NpgsqlParameter("l_m_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[30].Value = ruteAntall.Antall_L_M_RES;

                npgsqlParameters[31] = new NpgsqlParameter("antp_in", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[31].Value = ruteAntall.Antall_ANTP;

                npgsqlParameters[32] = new NpgsqlParameter("rute_dist_freq_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[32].Value = ruteAntall.RuteDistFreq;

                npgsqlParameters[33] = new NpgsqlParameter("sondagflag_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[33].Value = ruteAntall.SondagFlag;

                npgsqlParameters[34] = new NpgsqlParameter("p_hh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[34].Value = ruteAntall.Antall_NP_HH_U_RES;

                npgsqlParameters[35] = new NpgsqlParameter("p_vh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[35].Value = ruteAntall.Antall_NP_VH_U_RES;

                npgsqlParameters[36] = new NpgsqlParameter("np_hh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[36].Value = ruteAntall.Antall_NP_HH_U_RES;

                npgsqlParameters[37] = new NpgsqlParameter("np_vh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[37].Value = ruteAntall.Antall_NP_VH_U_RES;

                npgsqlParameters[38] = new NpgsqlParameter("p_hh_mm_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[38].Value = ruteAntall.Antall_P_HH_MM_RES;

                npgsqlParameters[39] = new NpgsqlParameter("p_vh_mm_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[39].Value = ruteAntall.Antall_P_VH_MM_RES;

                npgsqlParameters[40] = new NpgsqlParameter("np_hh_mm_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[40].Value = ruteAntall.Antall_NP_HH_MM_RES;

                npgsqlParameters[41] = new NpgsqlParameter("np_vh_mm_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[41].Value = ruteAntall.Antall_NP_VH_MM_RES;
                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.insertroutereolerdata", CommandType.StoredProcedure, npgsqlParameters);

                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Input_Ruter_Add_One:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from Input_Ruter_Add_One");
        }

        #endregion

        #region Route_Antall_Kommune
        /// <summary>
        /// Inputs the ruter kommune delete all.
        /// </summary>
        public async Task Input_Ruter_Kommune_Delete_All()
        {

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            int result;
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for Input_Ruter_Kommune_Delete_All");
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleteantallkommunedata", CommandType.StoredProcedure, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Input_Ruter_Kommune_Delete_All:", exception.Message);
                throw;
            }
            _logger.LogInformation("Result is : ", result);
            _logger.LogDebug("Exiting from Input_Ruter_Kommune_Delete_All");
        }

        /// <summary>
        /// Inputs the ruter kommune add one.
        /// </summary>
        /// <param name="rute_AntallKommune">The rute antall kommune.</param>
        public async Task Input_Ruter_Kommune_Add_One(Rute_AntallKommune rute_AntallKommune)
        {
           
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[26];
            int result;
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for Input_Ruter_Kommune_Add_One");
            #region Parameter assignement

            npgsqlParameters[0] = new NpgsqlParameter("reol_id_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = rute_AntallKommune.ReolID;

            npgsqlParameters[1] = new NpgsqlParameter("kommuneid_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = rute_AntallKommune.KommuneNr;

            npgsqlParameters[2] = new NpgsqlParameter("hh_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[2].Value = rute_AntallKommune.Antall_HH_U_RES;

            npgsqlParameters[3] = new NpgsqlParameter("er_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[3].Value = 0;

            npgsqlParameters[4] = new NpgsqlParameter("gb_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[4].Value = 0;

            npgsqlParameters[5] = new NpgsqlParameter("vh_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[5].Value = rute_AntallKommune.Antall_VH_U_RES;

            npgsqlParameters[6] = new NpgsqlParameter("hh_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[6].Value = rute_AntallKommune.Antall_HH_M_RES;

            npgsqlParameters[7] = new NpgsqlParameter("er_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[7].Value = 0;

            npgsqlParameters[8] = new NpgsqlParameter("gb_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[8].Value = 0;

            npgsqlParameters[9] = new NpgsqlParameter("reolnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[9].Value = rute_AntallKommune.ReolNr;

            npgsqlParameters[10] = new NpgsqlParameter("teamnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[10].Value = rute_AntallKommune.TeamNr;

            npgsqlParameters[11] = new NpgsqlParameter("vh_m_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[11].Value = rute_AntallKommune.Antall_VH_M_RES;

            npgsqlParameters[12] = new NpgsqlParameter("hh_gr_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[12].Value = rute_AntallKommune.Antall_HH_GR_RES;

            npgsqlParameters[13] = new NpgsqlParameter("l_u_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[13].Value = rute_AntallKommune.Antall_L_U_RES;

            npgsqlParameters[14] = new NpgsqlParameter("l_m_res_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[14].Value = rute_AntallKommune.Antall_L_M_RES;

            npgsqlParameters[15] = new NpgsqlParameter("antp_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[15].Value = rute_AntallKommune.Antall_ANTP;

            npgsqlParameters[16] = new NpgsqlParameter("rute_dist_frek_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[16].Value = rute_AntallKommune.RuteDistfreq;

            npgsqlParameters[17] = new NpgsqlParameter("sondag_flag_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[17].Value = rute_AntallKommune.SondagFlag;

            npgsqlParameters[18] = new NpgsqlParameter("p_hh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[18].Value = rute_AntallKommune.Antall_NP_HH_U_RES;

            npgsqlParameters[19] = new NpgsqlParameter("p_vh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[19].Value = rute_AntallKommune.Antall_NP_VH_U_RES;

            npgsqlParameters[20] = new NpgsqlParameter("np_hh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[20].Value = rute_AntallKommune.Antall_NP_HH_U_RES;

            npgsqlParameters[21] = new NpgsqlParameter("np_vh_u_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[21].Value = rute_AntallKommune.Antall_NP_VH_U_RES;

            npgsqlParameters[22] = new NpgsqlParameter("p_hh_m_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[22].Value = rute_AntallKommune.Antall_P_HH_M_RES;

            npgsqlParameters[23] = new NpgsqlParameter("p_vh_m_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[23].Value = rute_AntallKommune.Antall_P_HH_M_RES;

            npgsqlParameters[24] = new NpgsqlParameter("np_hh_m_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[24].Value = rute_AntallKommune.Antall_NP_HH_M_RES;

            npgsqlParameters[25] = new NpgsqlParameter("np_vh_m_res_in", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[25].Value = rute_AntallKommune.Antall_NP_VH_M_RES;
            #endregion
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.insertreolerkommunedata", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Input_Ruter_Kommune_Add_One:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from Input_Ruter_Kommune_Delete_All");
        }

        #endregion

        #region Route_Rutepunkter
        /// <summary>
        /// Inputs the relopunkter delete all.
        /// </summary>
        public async Task Input_Relopunkter_Delete_All()
        {
            await Task.Run(() => { });
            int result;
            _logger.LogDebug("Preparing the data for Input_Relopunkter_Delete_All");
            // await Task.Run(() => { });
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleterouterutepunkterdata", CommandType.StoredProcedure, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Input_Relopunkter_Delete_All:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from Input_Relopunkter_Delete_All");

        }

        /// <summary>
        /// Inputs the relopunkter add one.
        /// </summary>
        /// <param name="rutePunkter">The rute punkter.</param>
        public async Task Input_Relopunkter_Add_One(Rute_RutePunkter rutePunkter)
        {

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[24];
            int result;
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for Input_Relopunkter_Add_One");
            #region Parameter assignement

            npgsqlParameters[0] = new NpgsqlParameter("reol_id_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = rutePunkter.ReolID;

            npgsqlParameters[1] = new NpgsqlParameter("x_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[1].Value = rutePunkter.X;

            npgsqlParameters[2] = new NpgsqlParameter("y_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[2].Value = rutePunkter.Y;

            npgsqlParameters[3] = new NpgsqlParameter("adrnrgab_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[3].Value = rutePunkter.AdrNrGAB;

            npgsqlParameters[4] = new NpgsqlParameter("adrkategori_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[4].Value = rutePunkter.AdrKategori;

            npgsqlParameters[5] = new NpgsqlParameter("adrnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[5].Value = rutePunkter.AdrNr;

            npgsqlParameters[6] = new NpgsqlParameter("adrnrint_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[6].Value = "0";

            npgsqlParameters[7] = new NpgsqlParameter("kommunenr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[7].Value = rutePunkter.KommuneNr;

            npgsqlParameters[8] = new NpgsqlParameter("navn_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[8].Value = rutePunkter.NavnSted;

            npgsqlParameters[9] = new NpgsqlParameter("postnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[9].Value = rutePunkter.PostNr;

            //npgsqlParameters[10] = new NpgsqlParameter("teamnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[10].Value = rutePunkter.TeamNr;

            npgsqlParameters[10] = new NpgsqlParameter("kodegate_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[10].Value = DBNull.Value;

            npgsqlParameters[11] = new NpgsqlParameter("boksnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[11].Value = DBNull.Value;

            npgsqlParameters[12] = new NpgsqlParameter("husnr_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[12].Value = rutePunkter.HusNr;

            npgsqlParameters[13] = new NpgsqlParameter("bokstav_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[13].Value = rutePunkter.Bokstav;

            npgsqlParameters[14] = new NpgsqlParameter("oppgang_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[14].Value = rutePunkter.Oppgang;

            npgsqlParameters[15] = new NpgsqlParameter("gardnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[15].Value = DBNull.Value;

            npgsqlParameters[16] = new NpgsqlParameter("bruknr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[16].Value = DBNull.Value;

            npgsqlParameters[17] = new NpgsqlParameter("festenr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[17].Value = DBNull.Value;

            npgsqlParameters[18] = new NpgsqlParameter("undernr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[18].Value = DBNull.Value;

            npgsqlParameters[19] = new NpgsqlParameter("postadresse_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[19].Value = rutePunkter.PostAdresse;

            npgsqlParameters[20] = new NpgsqlParameter("adrnrstedadr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[20].Value = DBNull.Value;

            npgsqlParameters[21] = new NpgsqlParameter("rutedistfreq_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[21].Value = rutePunkter.RuteDistFreq;

            npgsqlParameters[22] = new NpgsqlParameter("sondagflag_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[22].Value = rutePunkter.SondagFlag;

            npgsqlParameters[23] = new NpgsqlParameter("priority_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[23].Value = rutePunkter.Priority;

            #endregion

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.insertroutereolpunkterdata", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception,exception.Message);
                    throw;
                }
            }
            _logger.LogDebug("Exiting from Input_Relopunkter_Add_One");

        }

        #endregion

        #region Route_Boksanlegg
        /// <summary>
        /// Inputs the boksanlegg delete all.
        /// </summary>
        public async Task Input_Boksanlegg_Delete_All()
        {

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            int result;
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for Input_Boksanlegg_Delete_All");
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleterouteboksanleggdata", CommandType.StoredProcedure, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Input_Boksanlegg_Delete_All:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from Input_Boksanlegg_Delete_All");
        }

        /// <summary>
        /// Inputs the boksanlegg add one.
        /// </summary>
        /// <param name="ruteBoksAnlegg">The rute boks anlegg.</param>
        public async Task Input_Boksanlegg_Add_One(Rute_BoksAnlegg ruteBoksAnlegg)
        {

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[15];
            int result;
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for Input_Boksanlegg_Add_One");
            #region Parameter assignement

            npgsqlParameters[0] = new NpgsqlParameter("reol_id_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = ruteBoksAnlegg.ReolID;

            npgsqlParameters[1] = new NpgsqlParameter("enhnrruteansv_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = ruteBoksAnlegg.TeamNr;

            npgsqlParameters[2] = new NpgsqlParameter("reolnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = ruteBoksAnlegg.ReolNr;

            npgsqlParameters[3] = new NpgsqlParameter("boksanlnr_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[3].Value = ruteBoksAnlegg.BoksAnlNr;

            npgsqlParameters[4] = new NpgsqlParameter("aktornrhent_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[4].Value = ruteBoksAnlegg.AktorNrHent;

            npgsqlParameters[5] = new NpgsqlParameter("navnvirksomhet_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[5].Value = ruteBoksAnlegg.NavnVirksomhet;

            npgsqlParameters[6] = new NpgsqlParameter("navngate_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[6].Value = ruteBoksAnlegg.NavnGate;

            npgsqlParameters[7] = new NpgsqlParameter("husnr_in", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[7].Value = ruteBoksAnlegg.HusNr;

            npgsqlParameters[8] = new NpgsqlParameter("bokst_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[8].Value = ruteBoksAnlegg.Bokstav;

            npgsqlParameters[9] = new NpgsqlParameter("postnr_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[9].Value = ruteBoksAnlegg.PostNr;

            npgsqlParameters[10] = new NpgsqlParameter("postadresse_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[10].Value = ruteBoksAnlegg.PostAdresse;

            npgsqlParameters[11] = new NpgsqlParameter("orgtype_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[11].Value = 35;

            npgsqlParameters[12] = new NpgsqlParameter("adrnrgab_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[12].Value = ruteBoksAnlegg.AdrNrGAB;

            npgsqlParameters[13] = new NpgsqlParameter("x_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[13].Value = ruteBoksAnlegg.X;

            npgsqlParameters[14] = new NpgsqlParameter("y_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[14].Value = ruteBoksAnlegg.Y;

            #endregion

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connectionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.insertrouteboksanleggdata", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                    throw;
                }
            }

        }

        #endregion
    }
}
