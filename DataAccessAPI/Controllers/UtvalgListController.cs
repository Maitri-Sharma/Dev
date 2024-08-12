#region Namespaces
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using DataAccessAPI.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Npgsql;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using static Puma.Shared.PumaEnum;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class UtvalgListController : ControllerBase
    {
        #region Variables
        private readonly ILogger<UtvalgListController> _logger;
        private readonly ConfigController configController;
        private readonly UtvalgController utvalgController;
        private readonly IUtvalgRepository _utvalgRepository;
        private readonly IMediator _mediator;
        #endregion

        #region Constructors       
        /// <summary>
        /// Initializes a new instance of the <see cref="UtvalgListController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="loggerUtvalg">The logger utvalg.</param>
        /// <param name="loggerreol">The loggerreol.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="mediator">The mediator.</param>
        public UtvalgListController(ILogger<UtvalgListController> logger, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol,
          IUtvalgRepository utvalgRepository, IMediator mediator = null)
        {
            _logger = logger;
            _utvalgRepository = utvalgRepository;
            configController = new ConfigController(loggerConfig);
            _mediator = mediator;

            utvalgController = new UtvalgController(loggerUtvalg, loggerConfig, loggerreol, _mediator);

        }
        #endregion

        #region Public Methods
        //public readonly string errMsgListName = "Listenavnet må ha minst 3 tegn.";
        //public readonly string errMsgIllegalCharsLst = "Listenavnet inneholder ulovlige tegn. Fjern tegnene '<' og '>' dersom eksisterer i navnet.";
        //public readonly string errMsgListNameWithSpaces = "Listenavnet kan ikke ha mellomrom i begynnelsen eller slutten av navnet. Fjern mellomrom og prøv på nytt.";

        //[HttpPost("SaveUtvalgList", Name = nameof(SaveUtvalgList))]
        //public void SaveUtvalgList([FromBody]UtvalgList list, string userName)
        //{
        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
        //    int result;
        //    if (list.Name.Trim().Length < 3)
        //        throw new Exception(errMsgListName);
        //    if ((list.Name.IndexOf(">") > -1 | list.Name.IndexOf("<") > -1))
        //        throw new Exception(errMsgIllegalCharsLst);
        //    if ((list.Name.IndexOf(" ", 0, 1) > -1 | list.Name.IndexOf(" ", list.Name.Length - 1, 1) > -1))
        //        throw new Exception(errMsgListNameWithSpaces);
        //    // If Not list.KundeNummer Is Nothing AndAlso list.KundeNummer.Trim() = "" Then Throw New Exception("Customer number '" & list.KundeNummer & "' is valid when saving an utvalgsliste!")
        //    try
        //    {
        //        // Finner evt parentlistid
        //        int ParentUtvalgListId = GetParentUtvalgListId(list.ListId);

        //        if (list.ListId == 0)
        //            // list.ListId = GetSequenceNextVal("KSPU_DB.UtvalgListId_Seq", trans);
        //            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
        //        npgsqlParameters[0].Value = list.ListId;

        //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
        //        {
        //            result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalglist", CommandType.StoredProcedure, npgsqlParameters);
        //        }

        //        // _logger.LogInformation("Number of row returned {0}", Convert.ToInt32(result));

        //        if (result == 0)
        //        {
        //            // Update affected no rows, we must do an insert
        //            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
        //            {
        //                result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalglist", CommandType.StoredProcedure, npgsqlParameters);
        //            }

        //            //cmd.CommandText = " INSERT INTO KSPU_DB.UtvalgList (UtvalgListID, KundeNummer, Logo, OrdreReferanse, OrdreType, OrdreStatus, InnleveringsDato, Antall, UtvalgListName, ParentUtvalgListId, Vekt, Distribusjonsdato, Distribusjonstype, IsBasis, BasedOn, WasBasedOn, AllowDouble, Thickness) "
        //            //    + " VALUES (:UtvalgListId, :KundeNummer, :Logo, :OrdreReferanse, :OrdreType, :OrdreStatus, :InnleveringsDato, :Antall, :UtvalgListName, :ParentUtvalgListId, :Vekt, :Distribusjonsdato, :Distribusjonstype, :IsBasis, :BasedOn, :WasBasedOn, :AllowDouble, :Thickness) ";
        //            //ExecuteNonQuery(cmd);
        //        }

        //        SaveUtvalgListModifications(list.ListId, userName, "SaveUtvalgList - ", list.Antall);

        //        // Check if list was connected to parent list, if so - update modification table
        //        if (list.ParentList != null)
        //            SaveUtvalgListModifications(list.ParentList.ListId, userName, "SaveUtvalgList Parent- ", list.Antall);

        //        if ((ParentUtvalgListId > 0))
        //            UpdateAntallInList(ParentUtvalgListId);

        //        if ((list.ParentList != null && list.ParentList.ListId != ParentUtvalgListId))
        //            UpdateAntallInList(list.ParentList.ListId);
        //        list.AntallWhenLastSaved = list.Antall;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        [HttpPost("SaveUtvalgList", Name = nameof(SaveUtvalgList))]
        [ProducesResponseType(typeof(ResponseSearchUtvalgListSimpleById), (int)HttpStatusCode.OK)]
        public IActionResult SaveUtvalgList([FromBody] RequestSaveUtvalgList list, string userName)
        {
            _logger.BeginScope("InSide Into SaveUtvalgList");
            list.userName = userName;
            return Ok(_mediator.Send(list).Result);
        }

        /// <summary>
        /// Creates the campaign list.
        /// </summary>
        /// <param name="listId">Id of list.</param>
        /// <param name="campaignName">Name of the campaign.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="antall">Antall of list.</param>
        /// <returns></returns>
        [HttpPost("CreateCampaignList", Name = nameof(CreateCampaignList))]
        [ProducesResponseType(typeof(ResponseSearchUtvalgListSimpleById), (int)HttpStatusCode.OK)]
        public IActionResult CreateCampaignList(int listId, string campaignName, string userName, double antall)
        {
            _logger.BeginScope("InSide Into CreateCampaignList");
            RequestCreateCampaignList list = new RequestCreateCampaignList();
            list.ListId = listId;
            list.BasedOn = listId;
            list.Name = campaignName;
            list.userName = userName;
            list.Antall = antall;
            return Ok(_mediator.Send(list).Result);
        }
        /// <summary>
        ///     ''' Search Utvalg List data by list ID
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <returns>UtvalgSearchCollection</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtvalgListSimple), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListSimple")]
        public IActionResult GetUtvalgListSimple(int listId)
        {
            _logger.BeginScope("InSide Into GetUtvalgListSimple");
            #region Old code
            //_logger.LogDebug("Inside into GetUtvalgListSimple");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //DataRow dataRow;


            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = listId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistsimple", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (utvData.Rows.Count != 1)
            //    throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            //dataRow = utvData.Rows[0];
            //_logger.LogInformation("Number of row returned: ", utvData.Rows.Count);
            //_logger.LogDebug("Exiting from GetUtvalgListSimple");

            //return CreateListFromRow(dataRow);
            #endregion
            RequestGetUtvalgListSimple request = new RequestGetUtvalgListSimple()
            {
                listId = listId
            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Search Utvalg List with children by ID
        /// </summary>
        /// <param name="utvalglistId"></param>
        /// <param name="includeReols"></param>    
        /// <returns>UtvalgSearchCollection</returns>

        //[HttpGet]
        //[ProducesResponseType(typeof(ResponseSearchUtvalgListWithChildrenById), (int)HttpStatusCode.OK)]
        //[Route("SearchUtvalgListWithChildrenById")]
        [HttpGet("SearchUtvalgListWithChildrenById", Name = nameof(SearchUtvalgListWithChildrenById))]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListWithChildrenById>), (int)HttpStatusCode.OK)]
        public IActionResult SearchUtvalgListWithChildrenById(int utvalglistId, bool includeReols = true)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListWithChildrenById");
            #region Old code
            //_logger.LogDebug("Inside into SearchUtvalgListWithChildrenById");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //UtvalgsListCollection utvLColl;


            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalglistId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistwithchildrenbyid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //utvLColl = GetAllListContent(utvData, includeReols, false);
            //AddSistOppdatertToLists(utvLColl);
            //_logger.LogInformation("Number of row returned: ", utvLColl.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgListWithChildrenById");

            //return utvLColl;
            #endregion

            RequestSearchUtvalgListWithChildrenById request = new RequestSearchUtvalgListWithChildrenById()
            {
                listId = utvalglistId

            };

            return Ok(_mediator.Send(request).Result);

        }

        /// <summary>
        ///  Sett nyeste dato for sist innholdet i en liste ble oppdatert, og hvem som utførte endringen
        ///  </summary>
        ///  <param name="utvalgsListCollection"></param>
        ///  <remarks></remarks>
        [HttpGet("AddSistOppdatertToLists", Name = nameof(AddSistOppdatertToLists))]
        public void AddSistOppdatertToLists(UtvalgsListCollection utvalgsListCollection)
        {
            _logger.BeginScope("InSide Into AddSistOppdatertToLists");
            string sistEndretAv = "";
            foreach (UtvalgList list in utvalgsListCollection)
            {

                // sjekker listens egne datoer også
                DateTime sistListOppdatert = new DateTime(1, 1, 1);
                if (list.Modifications == null)
                {
                    list.Modifications = new List<UtvalgModification>();
                    GetUtvalgListModifications(list);
                }
                if (list.Modifications != null)
                {
                    foreach (UtvalgModification modification in list.Modifications)
                    {
                        if (modification.ModificationTime.CompareTo(sistListOppdatert) > 0)
                        {
                            sistListOppdatert = modification.ModificationTime;
                            sistEndretAv = modification.UserId;
                        }
                    }
                }
                list.SistOppdatert = sistListOppdatert;
                list.SistEndretAv = sistEndretAv;

                // sjekker listens utvalg
                DateTime sistOppdatert = new DateTime(1, 1, 1);
                foreach (Utvalg utvalg in list.MemberUtvalgs)
                {
                    if (utvalg.Modifications != null)
                    {
                        foreach (UtvalgModification modification in utvalg.Modifications)
                        {
                            if (modification.ModificationTime.CompareTo(sistOppdatert) > 0)
                            {
                                sistOppdatert = modification.ModificationTime;
                                sistEndretAv = modification.UserId;
                            }
                        }
                    }
                }
                if (sistOppdatert.CompareTo(list.SistOppdatert) > 0)
                {
                    list.SistOppdatert = sistOppdatert;
                    list.SistEndretAv = sistEndretAv;
                }

                // childlist
                foreach (UtvalgList childlist in list.MemberLists)
                {
                    sistOppdatert = new DateTime(1, 1, 1);
                    if (childlist.MemberUtvalgs.Count == 0)
                        childlist.MemberUtvalgs = _utvalgRepository.SearchUtvalgByUtvalListId(childlist.ListId, false).Result;
                    foreach (Utvalg utvalg in childlist.MemberUtvalgs)
                    {
                        if (utvalg.Modifications != null)
                        {
                            foreach (UtvalgModification modification in utvalg.Modifications)
                            {
                                if (modification.ModificationTime.CompareTo(sistOppdatert) > 0)
                                {
                                    sistOppdatert = modification.ModificationTime;
                                    sistEndretAv = modification.UserId;
                                }
                            }
                        }
                    }

                    // sjekk underlistens egen dato også
                    if (childlist.Modifications == null)
                    {
                        childlist.Modifications = new List<UtvalgModification>();
                        GetUtvalgListModifications(childlist);
                    }
                    if (childlist.Modifications != null)
                    {
                        foreach (UtvalgModification modification in childlist.Modifications)
                        {
                            if (modification.ModificationTime.CompareTo(sistOppdatert) > 0)
                            {
                                sistOppdatert = modification.ModificationTime;
                                sistEndretAv = modification.UserId;
                            }
                        }
                    }

                    childlist.SistOppdatert = sistOppdatert;
                    childlist.SistEndretAv = sistEndretAv;
                    if (childlist.SistOppdatert.CompareTo(list.SistOppdatert) > 0)
                    {
                        list.SistOppdatert = childlist.SistOppdatert;
                        list.SistEndretAv = childlist.SistEndretAv;
                    }
                }
            }
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med KundeNummer.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - alle childlister
        ///     '''  - alle utvalg som tilhører childlisten
        ///     ''' 
        ///     '''  En liste kan inneholde en liste. Max 2 nivåer med lister. Liste i liste.
        ///     '''  Dvs enten har lista ingen parent eller children, eller den har enten en parent eller children..
        ///     ''' </summary>
        ///     ''' <param name="kundeNummer"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <param name="includeReols"></param>
        ///     ''' <returns>UtvalgsListCollection</returns>
        ///     ''' <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListWithChildrenByKundeNummer>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListWithChildrenByKundeNummer")]
        public IActionResult SearchUtvalgListWithChildrenByKundeNummer(string kundeNummer, SearchMethod searchMethod, bool includeReols = true, bool onlyBasisUtvalglist = false)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListWithChildrenByKundeNummer");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgListWithChildrenByKundeNummer");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //UtvalgsListCollection utvLColl;
            //Utils utils = new Utils();

            //npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = kundeNummer;

            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistwithchildrenbykundeNummer", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //utvLColl = GetAllListContent(utvData, includeReols, false);
            //AddSistOppdatertToLists(utvLColl);
            //_logger.LogInformation("Number of row returned: ", utvLColl.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgListWithChildrenByKundeNummer");

            //return utvLColl; 
            #endregion
            RequestSearchUtvalgListWithChildrenByKundeNummer request = new RequestSearchUtvalgListWithChildrenByKundeNummer()
            {
                searchMethod = searchMethod,
                includeReols = includeReols,
                kundeNummer = kundeNummer,
                onlyBasisUtvalglist = onlyBasisUtvalglist

            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Gets a list of the reol ids in the UtvalgReoltable for a given utvalglist.
        ///     ''' Includes all childslists utvalg
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        //[HttpGet("GetUtvalgListReolIDs", Name = nameof(GetUtvalgListReolIDs))]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtvalgListReolIDs), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListReolIDs")]
        public IActionResult GetUtvalgListReolIDs(int listId)
        {
            _logger.BeginScope("InSide Into GetUtvalgListReolIDs");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgListReolIDs");
            //DataTable reolIds;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //List<long> result = new List<long>();
            //Utils utils = new Utils();


            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = listId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolIds = dbhelper.FillDataTable("kspu_db.getutvalglistreolids", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in reolIds.Rows)
            //    //result.Add((long)row["r_reolid"]);
            //    result.Add(Convert.ToInt64(utils.GetLongFromRow(row,"r_reolid")));

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from GetUtvalgListReolIDs");

            //return result;
            #endregion  OldCode
            RequestGetUtvalgListReolIDs request = new RequestGetUtvalgListReolIDs()
            {
                listId = listId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///  To check if utvalg List exists
        ///     ''' </summary>
        ///     ''' <param name="utvalglistname"></param>
        ///     ''' <returns>True or False</returns>
        ///     ''' <remarks></remarks>

        //[HttpGet("UtvalgListNameExists", Name = nameof(UtvalgListNameExists))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UtvalgListNameExists")]
        public IActionResult UtvalgListNameExists(string utvalglistname)
        {
            _logger.BeginScope("InSide Into UtvalgListNameExists");
            #region Old Code
            //_logger.LogDebug("Inside into UtvalgListNameExists");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utvalglistname;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.utvalglistnameexists", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from UtvalgListNameExists");
            //if (result == null | (result) is DBNull)
            //    return false;
            //return System.Convert.ToInt32(result) > 0;
            #endregion 
            RequestUtvalgListNameExists request = new RequestUtvalgListNameExists()
            {
                utvalglistname = utvalglistname
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Checks if a list can be connected to the specified parent list. Returns nothing if both:
        ///     ''' * The list has no member lists.
        ///     ''' * The parent list has no parent list.
        ///     ''' 
        ///     ''' This is to avoid structures with more than two levels of lists.
        ///     ''' 
        ///     ''' Otherwise returns an error message.
        ///     ''' </summary>
        ///     ''' <param name="listID"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        //[HttpGet("ListHasMemberLists", Name = nameof(ListHasMemberLists))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("ListHasMemberLists")]
        public IActionResult ListHasMemberLists(int listID)
        {
            _logger.BeginScope("InSide Into ListHasMemberLists");
            #region Old Code
            //_logger.LogDebug("Inside into ListHasMemberLists");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = listID;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.ListHasMemberLists", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from ListHasMemberLists");
            //if ((result) is decimal)
            //{
            //    if (System.Convert.ToInt32(result) > 0)
            //        return true;
            //}
            //return false;
            #endregion
            RequestListHasMemberLists request = new RequestListHasMemberLists()
            {
                listId = listID
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Returns the id of the given list's parent, or -1 if the list has no parent.
        ///     ''' </summary>
        ///     ''' <param name="utvalgListId"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        [HttpGet("GetParentUtvalgListId", Name = nameof(GetParentUtvalgListId))]
        public int GetParentUtvalgListId(int utvalgListId)
        {
            _logger.BeginScope("InSide Into GetParentUtvalgListId");
            #region Old Code
            _logger.LogDebug("Inside into GetParentUtvalgListId");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result; ;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgListId;
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<int>("kspu_db.getparentutvalglistid", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Number of row returned: ", result);
            _logger.LogDebug("Exiting from GetParentUtvalgListId");

            if (result == DBNull.Value)
                return -1;
            return System.Convert.ToInt32(result);
            #endregion 

        }

        /// <summary>
        ///  To check if list has Parent list or not
        ///     ''' </summary>
        ///     ''' <param name="utvalgListId"></param>
        ///     ''' <returns>True or False</returns>
        ///     ''' <remarks></remarks>
        [HttpGet("ListHasParentList", Name = nameof(ListHasParentList))]
        public bool ListHasParentList(int utvalgListId)
        {
            return GetParentUtvalgListId(utvalgListId) > 0;
        }

        /// <summary>
        ///     ''' Search Utvalg List by user ID
        ///     ''' </summary>
        ///     ''' <param name="userID"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <returns>UtvalgSearchCollection</returns>
        //[HttpGet("SearchUtvalgListByUserID", Name = nameof(SearchUtvalgListByUserID))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListByUserID>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListByUserID")]
        public IActionResult SearchUtvalgListByUserID(string userID, SearchMethod searchMethod)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListByUserID");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgListByUserID");
            //DataTable utvLData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //UtvalgsListCollection utvLColl = new UtvalgsListCollection();


            //npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            ////npgsqlParameters[0].Value = utils.CreateSearchString(userID, searchMethod);
            //npgsqlParameters[0].Value = userID;
            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistbyuserid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvLData.Rows)
            //    utvLColl.Add(CreateListFromRow(row));

            //_logger.LogInformation("Number of row returned: ", utvLColl);
            //_logger.LogDebug("Exiting from SearchUtvalgListByUserID");
            //return utvLColl;
            #endregion Old Code
            RequestSearchUtvalgListByUserID request = new RequestSearchUtvalgListByUserID()
            {
                UserId = userID,
                SearchMethod = searchMethod
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med navn.
        ///     '''  Metoden returnerer:
        ///     '''  - alle lister etter søkekriteriet 
        ///     ''' </summary>
        ///     ''' <param name="utvalglistname"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListSimple>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListSimple")]
        public IActionResult SearchUtvalgListSimple(string utvalglistname, SearchMethod searchMethod)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListSimple");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgListSimple");
            //DataTable utvLData;
            //Utils utils = new Utils();
            //UtvalgList utvalgList = new UtvalgList();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            ////npgsqlParameters[0].Value = utils.CreateSearchString(utvalglistname, searchMethod);
            //npgsqlParameters[0].Value = utvalglistname;

            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimple", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvLData.Rows)
            //    utvLColl.Add(CreateListFromRow(row));

            //_logger.LogInformation("Number of row returned: ", utvLColl);
            //_logger.LogDebug("Exiting from SearchUtvalgListSimple");
            //return utvLColl;
            #endregion Old Code
            RequestSearchUtvalgListSimple request = new RequestSearchUtvalgListSimple()
            {
                utvalglistname = utvalglistname,
                searchMethod = searchMethod
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        ///     '''  Søker etter utvalgslister med navn.
        ///     '''  Metoden returnerer:
        ///     '''  - alle lister etter søkekriteriet 
        ///     ''' </summary>
        ///     ''' <param name="utvalglistname"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <param name="onlyBasisLists"></param>
        ///     ''' <param name="isBasedOn">Set false when do not want campain list in result</param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListSimple>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListSimpleByIsBasis")]
        public IActionResult SearchUtvalgListSimpleByIsBasis(string utvalglistname, SearchMethod searchMethod, int onlyBasisLists, bool isBasedOn = true)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListSimpleByIsBasis");

            RequestSearchUtvalgListByIsBasis request = new RequestSearchUtvalgListByIsBasis()
            {
                utvalglistname = utvalglistname,
                searchMethod = searchMethod,
                onlyBasisLists = onlyBasisLists,
                isBasedOn = isBasedOn
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// Søker etter utvalgslister med id.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - parentlisten
       // [HttpGet("GetUtvalgList", Name = nameof(GetUtvalgList))]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtvalgList), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgList")]
        public IActionResult GetUtvalgList(int listId, bool getParentList = true, bool getMemberUtvalg = false)
        {
            _logger.BeginScope("InSide Into GetUtvalgList");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgList");
            //DataTable utvLData;
            //UtvalgList list;
            //DataRow row;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];


            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = listId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglist", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (utvLData.Rows.Count != 1)
            //    throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            //row = utvLData.Rows[0];
            //list = CreateListFromRow(row);
            //_logger.LogInformation("Number of row returned: ", list);
            //_logger.LogDebug("Exiting from GetUtvalgList");
            //return list;
            #endregion Old Code

            RequestGetUtvalgList request = new RequestGetUtvalgList()
            {
                listId = listId,
                getMemberUtvalg = getMemberUtvalg,
                getParentList = getParentList
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Used to get listinfo for a 'arvingliste'. Does get the actual list, not swapping by arvingliste!
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        //[HttpGet("GetUtvalgListNoChild", Name = nameof(GetUtvalgListNoChild))]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtvalgListNoChild), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListNoChild")]
        public IActionResult GetUtvalgListNoChild(int listId)
        {
            _logger.BeginScope("InSide Into GetUtvalgListNoChild");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgListNoChild");
            //DataTable utvLData;
            //UtvalgList list;
            //DataRow row;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = listId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglistnochild", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (utvLData.Rows.Count != 1)
            //    throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            //row = utvLData.Rows[0];
            //list = CreateListFromRow(row);
            //_logger.LogInformation("Number of row returned: ", list);
            //_logger.LogDebug("Exiting from GetUtvalgListNoChild");
            //return list;
            #endregion Old Code
            RequestGetUtvalgListNoChild request = new RequestGetUtvalgListNoChild()
            {
                listId = listId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' For integration: Get all distinct PRS from reoler in Utvalg from selected Utvalglist and its childlist.
        ///     ''' Uses current ReolMap, independent of recreation done, needed or not.
        ///     ''' </summary>
        ///     ''' <param name="utvalglistId"></param>
        ///     ''' <remarks></remarks>
        //[HttpGet("GetDistinctPRSByListID", Name = nameof(GetDistinctPRSByListID))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("GetDistinctPRSByListID")]
        public IActionResult GetDistinctPRSByListID(long utvalglistId)
        {
            _logger.BeginScope("InSide Into GetDistinctPRSByListID");
            #region Old Code
            //_logger.LogDebug("Inside into GetDistinctPRSByListID");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //List<string> result = new List<string>();
            //string table = configController.CurrentReolTableName;
            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalglistId;

            //npgsqlParameters[1] = new NpgsqlParameter("p_table", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = table;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getdistinctprsbylistid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //{
            //    string s = row[0].ToString();
            //    result.Add(s);
            //}

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from GetDistinctPRSByListID");
            //return result;
            #endregion Old Code
            RequestGetDistinctPRSByListID request = new RequestGetDistinctPRSByListID()
            {
                utvalglistId = (int)utvalglistId,
                CurrentReolTableName = configController.CurrentReolTableName
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med id.
        ///     '''  Metoden returnerer:
        ///     '''  - alle lister etter søkekriteriet 
        ///     ''' </summary>
        ///     ''' <param name="utvalglistid"></param>
        ///     ''' <param name="customerNos"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
       // [HttpGet("SearchUtvalgListSimpleById", Name = nameof(SearchUtvalgListSimpleById))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListSimpleById>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListSimpleById")]
        public IActionResult SearchUtvalgListSimpleById(int utvalglistid, string customerNos = "")
        {
            _logger.BeginScope("InSide Into SearchUtvalgListSimpleById");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgListSimpleById");
            //DataTable utvLData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalglistid;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvLData.Rows)
            //    utvLColl.Add(CreateListFromRow(row));
            //AddSistOppdatertToLists(utvLColl);
            //_logger.LogInformation("Number of row returned: ", utvLColl.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgListSimpleById");
            //return utvLColl;
            #endregion Old Code

            RequestSearchUtvalgListSimpleById request = new RequestSearchUtvalgListSimpleById()
            {
                utvalglistid = utvalglistid,
                customerNos = customerNos
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        /// SearchUtvalgListSimpleByIdAndCustNoAgreeNo
        /// </summary>
        /// <param name="utvalglistid"></param>
        /// <param name="customerNos"></param>
        /// <param name="agreementNos"></param>
        /// <param name="forceCustomerAndAgreementCheck"></param>
        /// <param name="extendedInfo"></param>
        /// <param name="onlyBasisLists"></param>
        /// <param name="includeChildrenUtvalg"></param>
        /// <returns></returns>
        //[HttpGet("SearchUtvalgListSimpleByIdAndCustNoAgreeNo", Name = nameof(SearchUtvalgListSimpleByIdAndCustNoAgreeNo))]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseSearchUtvalgListSimpleByIdAndCustNoAgreeNo), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListSimpleByIdAndCustNoAgreeNo")]
        public IActionResult SearchUtvalgListSimpleByIdAndCustNoAgreeNo(int utvalglistid, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisLists, bool includeChildrenUtvalg = false)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListSimpleByIdAndCustNoAgreeNo");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgListSimpleByIdAndCustNoAgreeNo");
            //DataTable utvData;
            ////bool abortSearch = false;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            //UtvalgsListCollection result = new UtvalgsListCollection();
            //Utils utils = new Utils();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters[0].Value = Convert.ToInt32(utvalglistid);

            //npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = "'" + String.Join("', '", customerNos) + "'";
            ////npgsqlParameters[1].Value = utils.GetCustomerNoSearchString(customerNos, searchMethodCusomerNos);

            //npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = "'" + String.Join("', '", agreementNos) + "'";
            ////npgsqlParameters[2].Value = utils.GetAgreementNoSearchString(agreementNos);

            //npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[3].Value = onlyBasisLists == 0 ? "0,1" : "1";


            //if (forceCustomerAndAgreementCheck)
            //{
            //    if (utils.CanSearch(customerNos, agreementNos))
            //    {
            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyidandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //        foreach (DataRow row in utvData.Rows)
            //        {
            //            UtvalgList l = CreateListFromRow(row);
            //            // add last saved by user if extendedInfo= true
            //            if (extendedInfo)
            //            {
            //                l.Modifications = new List<UtvalgModification>();
            //                GetUtvalgListModifications(l);
            //            }
            //            result.Add(l);
            //        }
            //    }
            //    //else
            //    //{
            //    //    abortSearch = true;
            //    //    return result;
            //    //}
            //}
            //else
            //{
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyidandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    foreach (DataRow row in utvData.Rows)
            //    {
            //        UtvalgList l = CreateListFromRow(row);
            //        // add last saved by user if extendedInfo= true
            //        if (extendedInfo)
            //        {
            //            l.Modifications = new List<UtvalgModification>();
            //            GetUtvalgListModifications(l);
            //        }
            //        result.Add(l);
            //    }
            //}

            //AddSistOppdatertToLists(result);

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgListSimpleByIdAndCustNoAgreeNo");
            //return result;

            #endregion Old Code
            RequestSearchUtvalgListSimpleByIdAndCustNoAgreeNo request = new RequestSearchUtvalgListSimpleByIdAndCustNoAgreeNo()
            {
                utvalglistid = utvalglistid,
                customerNos = customerNos,
                agreementNos = agreementNos,
                forceCustomerAndAgreementCheck = forceCustomerAndAgreementCheck,
                extendedInfo = extendedInfo,
                onlyBasisLists = onlyBasisLists,
                includeChildrenUtvalg = includeChildrenUtvalg
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Search by utvalg list name and/or customer number and agreement number, and you may only select Basisutvalg
        ///     ''' </summary>
        ///     ''' <param name="utvalglistname"></param>
        ///     ''' <param name="customerNos"></param>
        ///     ''' <param name="agreementNos"></param>
        ///     ''' <param name="forceCustomerAndAgreementCheck"></param>
        ///     ''' <param name="extendedInfo"></param>
        ///     ''' <param name="onlyBasisLists"></param>
        ///     ''' <param name="includeChildrenUtvalg"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        //[HttpGet("SearchUtvalgListSimpleByIDAndCustomerNo", Name = nameof(SearchUtvalgListSimpleByIDAndCustomerNo))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListSimpleByIDAndCustomerNo>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListSimpleByIDAndCustomerNo")]
        public IActionResult SearchUtvalgListSimpleByIDAndCustomerNo(string utvalglistname, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisLists, bool includeChildrenUtvalg)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListSimpleByIDAndCustomerNo");
            #region Old Code
            ////need to check
            //_logger.LogDebug("Inside into SearchUtvalgListSimpleByIDAndCustomerNo");
            //DataTable utvData;
            ////bool abortSearch = false;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            //UtvalgsListCollection result = new UtvalgsListCollection();
            //Utils utils = new Utils();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //if (utvalglistname != null)
            //{
            //    npgsqlParameters[0].Value = "%" + utvalglistname + "%";
            //}
            //else
            //{
            //    npgsqlParameters[0].Value = "";
            //}
            ////npgsqlParameters[0].Value = "%" + utvalglistname + "%";
            ////npgsqlParameters[0].Value = utils.CreateSearchString(utvalglistname, searchMethodUtvalgList);

            //npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = customerNos.Length > 0 ? "'" + String.Join("', '", customerNos) + "'" : "";
            ////npgsqlParameters[1].Value = utils.GetCustomerNoSearchString(customerNos, searchMethodCusomerNos);

            //npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = agreementNos.Length > 0 ? "" + String.Join(", ", agreementNos) + "" : "";
            ////npgsqlParameters[2].Value = utils.GetAgreementNoSearchString(agreementNos);

            //npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[3].Value = onlyBasisLists == 0 ? "0,1" : "1";


            //if (forceCustomerAndAgreementCheck)
            //{
            //    if (utils.CanSearch(customerNos, agreementNos))
            //    {
            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyutvalgnameandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //        foreach (DataRow row in utvData.Rows)
            //        {
            //            UtvalgList l = CreateListFromRow(row);
            //            // add last saved by user if extendedInfo= true
            //            if (extendedInfo)
            //            {
            //                l.Modifications = new List<UtvalgModification>();
            //                GetUtvalgListModifications(l);
            //            }

            //            if (includeChildrenUtvalg)
            //            {
            //                // tilhørende utvalg til lista
            //                UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(l.ListId, false).Result;
            //                foreach (Utvalg u in uc)
            //                    l.MemberUtvalgs.Add(u);
            //            }
            //            result.Add(l);
            //        }
            //    }
            //    //else
            //    //{
            //    //    abortSearch = true;
            //    //    // return result;
            //    //}
            //}
            //else
            //{
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyutvalgnameandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    foreach (DataRow row in utvData.Rows)
            //    {
            //        UtvalgList l = CreateListFromRow(row);
            //        // add last saved by user if extendedInfo= true
            //        if (extendedInfo)
            //        {
            //            l.Modifications = new List<UtvalgModification>();
            //            GetUtvalgListModifications(l);
            //        }

            //        if (includeChildrenUtvalg)
            //        {
            //            // tilhørende utvalg til lista
            //            UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(l.ListId, false).Result;
            //            foreach (Utvalg u in uc)
            //                l.MemberUtvalgs.Add(u);
            //        }
            //        result.Add(l);
            //    }
            //}

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgListSimpleByIDAndCustomerNo");
            //return result;

            #endregion Old Code
            RequestSearchUtvalgListSimpleByIDAndCustomerNo request = new RequestSearchUtvalgListSimpleByIDAndCustomerNo()
            {
                utvalglistname = utvalglistname,
                customerNos = customerNos,
                agreementNos = agreementNos,
                forceCustomerAndAgreementCheck = forceCustomerAndAgreementCheck,
                extendedInfo = extendedInfo,
                onlyBasisLists = onlyBasisLists,
                includeChildrenUtvalg = includeChildrenUtvalg
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Søker etter utvalgslister med OrdreReferanse og OEBSType.
        /// Metoden returnerer:
        /// - alle lister etter søkekriteriet 
        /// </summary>
        /// <param name="OrdreReferanse"></param>
        /// <param name="OrdreType"></param>
        /// <param name="searchMethod"></param>
        /// <returns>UtvalgsListCollection</returns>
        /// <remarks></remarks>
        //[HttpGet("SearchUtvalgListSimpleByOrdreReferanse", Name = nameof(SearchUtvalgListSimpleByOrdreReferanse))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListSimpleByOrdreReferanse>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListSimpleByOrdreReferanse")]
        public IActionResult SearchUtvalgListSimpleByOrdreReferanse(string OrdreReferanse, string OrdreType, SearchMethod searchMethod)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListSimpleByOrdreReferanse");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgListSimpleByOrdreReferanse");
            //DataTable utvLData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            //UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            //npgsqlParameters[0] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            ////npgsqlParameters[0].Value = utils.CreateSearchString(OrdreReferanse, searchMethod);
            //npgsqlParameters[0].Value = Convert.ToString(OrdreReferanse);

            //npgsqlParameters[1] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
            ////npgsqlParameters[0].Value = utils.CreateSearchString(OrdreReferanse, searchMethod);
            //npgsqlParameters[1].Value = Convert.ToString(OrdreType);

            //npgsqlParameters[2] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 10);
            //npgsqlParameters[2].Value = Convert.ToInt32(searchMethod);

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyordrereferanse", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow dataRow in utvLData.Rows)
            //    utvLColl.Add(CreateListFromRow(dataRow));

            //_logger.LogInformation("Number of row returned: ", utvLColl.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgListSimpleByOrdreReferanse");
            //return utvLColl;
            #endregion Old Code
            RequestSearchUtvalgListSimpleByOrdreReferanse request = new RequestSearchUtvalgListSimpleByOrdreReferanse()
            {
                OrdreReferanse = OrdreReferanse,
                OrdreType = OrdreType,
                SearchMethod = searchMethod
            };
            return Ok(_mediator.Send(request).Result);

        }


        /// <summary>
        ///     ''' Get unique Receivers for a list. 
        ///     ''' Gets only distinct 'ReceiverId'. Field 'Selected' not returned and is always set to False(I think its not in use at all..)
        ///     ''' Checking all chils list and utvalg
        ///     ''' </summary>
        ///     ''' <param name="listid"></param>
        ///     ''' <remarks></remarks>
        //[HttpGet("GetUtvalgListReceivers", Name = nameof(GetUtvalgListReceivers))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtvalgListReceivers>), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListReceivers")]
        public IActionResult GetUtvalgListReceivers(int listid)
        {
            _logger.BeginScope("InSide Into GetUtvalgListReceivers");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgListReceivers");
            //DataTable utvLData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //UtvalgReceiverList result = new UtvalgReceiverList();
            //UtvalgReceiver utvalgReceiver = new UtvalgReceiver();

            //npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Bigint);
            //npgsqlParameters[0].Value = listid;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglistreceivers", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow dataRow in utvLData.Rows)
            //{
            //    utvalgReceiver.ReceiverId = (ReceiverType)utils.GetEnumFromRow(dataRow, "r_receiverid", typeof(ReceiverType));
            //    utvalgReceiver.Selected = false;
            //    result.Add(utvalgReceiver);
            //}

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from GetUtvalgListReceivers");
            //return result;
            #endregion Old Code
            RequestGetUtvalgListReceivers request = new RequestGetUtvalgListReceivers()
            {
                listId = listid
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        ///     ''' Metoden oppdaterer utvalglista i henhold til krav i integrasjon mot Ordre2
        ///     ''' </summary>
        ///     ''' <param name="utvalgList"></param>
        ///     ''' <param name="username"></param>
        [HttpPut("UpdateUtvalgListForIntegration", Name = nameof(UpdateUtvalgListForIntegration))]
        public IActionResult UpdateUtvalgListForIntegration(UtvalgList utvalgList, string username)
        {
            _logger.BeginScope("InSide Into UpdateUtvalgListForIntegration");
            return Ok();
            #region Old Code
            //try
            //{
            //    _logger.LogDebug("Inside into UpdateUtvalgListForIntegration");

            //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[11];
            //    int result;
            //    int result2;
            //    #region Parameter assignement

            //    npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            //    npgsqlParameters[0].Value = utvalgList.ListId;

            //    npgsqlParameters[1] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 30);
            //    npgsqlParameters[1].Value = utvalgList.OrdreReferanse;

            //    npgsqlParameters[2] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreType.Null);
            //    npgsqlParameters[2].Value = utvalgList.OrdreType.ToString();

            //    npgsqlParameters[3] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreType.Null);
            //    npgsqlParameters[3].Value = utvalgList.OrdreStatus.ToString(); // Add parameter in Enu type

            //    npgsqlParameters[4] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.Timestamp);
            //    npgsqlParameters[4].Value = utvalgList.InnleveringsDato;

            //    npgsqlParameters[5] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[5].Value = utvalgList.Weight;

            //    npgsqlParameters[6] = new NpgsqlParameter("p_distributiondate", NpgsqlTypes.NpgsqlDbType.Timestamp);
            //    npgsqlParameters[6].Value = utvalgList.DistributionDate;

            //    npgsqlParameters[7] = new NpgsqlParameter("p_distributiontype", NpgsqlTypes.NpgsqlDbType.Char, (int)DistributionType.Null);
            //    npgsqlParameters[7].Value = utvalgList.DistributionType.ToString();

            //    npgsqlParameters[8] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            //    npgsqlParameters[8].Value = utvalgList.Thickness;

            //    npgsqlParameters[9] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Bigint);
            //    npgsqlParameters[9].Value = utvalgList.Avtalenummer;

            //    npgsqlParameters[10] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[10].Direction = ParameterDirection.Output;
            //    #endregion

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.updateutvalglistforintegration", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    if (result == 0)
            //        _logger.LogWarning("Integrasjon gjennom Webservice metode 'Ordrestatus' forsøkte å oppdatere et utvalg som ikke eksisterer i KSPU. Utvalgsid: " + utvalgList.ListId);
            //    if ((utvalgList.BasedOn == 0 && !utvalgList.IsBasis))
            //    {
            //        npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            //        npgsqlParameters[0].Value = utvalgList.ListId;

            //        npgsqlParameters[1] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 30);
            //        npgsqlParameters[1].Value = utvalgList.OrdreReferanse;

            //        npgsqlParameters[2] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
            //        npgsqlParameters[2].Value = utvalgList.OrdreType.ToString();


            //        npgsqlParameters[3] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreStatus.Null);
            //        npgsqlParameters[3].Value = utvalgList.OrdreStatus.ToString();

            //        npgsqlParameters[4] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.Timestamp);
            //        npgsqlParameters[4].Value = utvalgList.InnleveringsDato;

            //        npgsqlParameters[5] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
            //        npgsqlParameters[5].Value = utvalgList.Weight;

            //        npgsqlParameters[6] = new NpgsqlParameter("p_distributiondate", NpgsqlTypes.NpgsqlDbType.Timestamp);
            //        npgsqlParameters[6].Value = utvalgList.DistributionDate;

            //        npgsqlParameters[7] = new NpgsqlParameter("p_distributiontype", NpgsqlTypes.NpgsqlDbType.Varchar, (int)DistributionType.Null);
            //        npgsqlParameters[7].Value = utvalgList.DistributionType.ToString();

            //        npgsqlParameters[8] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            //        npgsqlParameters[8].Value = utvalgList.Thickness;

            //        npgsqlParameters[9] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Integer);
            //        npgsqlParameters[9].Value = utvalgList.Avtalenummer;

            //        npgsqlParameters[10] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            //        npgsqlParameters[10].Direction = ParameterDirection.Output;

            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            result2 = dbhelper.ExecuteNonQuery("kspu_db.updateutvalglistforintegrationfromutvalg", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //        if (result2 == 0)
            //            // Update affected no rows, list does not exist
            //            _logger.LogWarning("Integrasjon gjennom Webservice metode 'Ordrestatus' forsøkte å oppdatere en liste som ikke eksisterer i KSPU. Utvalgsid: " + utvalgList.ListId);
            //        // update modificated-by for utvalg. Need loop since utvalg
            //        // mainUtvalg
            //        if (utvalgList.MemberUtvalgs != null)
            //        {
            //            foreach (Utvalg u in utvalgList.MemberUtvalgs)
            //                utvalgController.SaveUtvalgModifications(u, username, "UpdateUtvalgListForIntegration - MedlemsUtvalg: ");
            //        }
            //        // utvalg in childList
            //        if (utvalgList.MemberLists != null)
            //        {
            //            foreach (UtvalgList lChild in utvalgList.MemberLists)
            //            {
            //                if (lChild.MemberUtvalgs != null)
            //                {
            //                    foreach (Utvalg uChild in lChild.MemberUtvalgs)
            //                        utvalgController.SaveUtvalgModifications(uChild, username, "UpdateUtvalgListForIntegration - utvalg i underlister:");
            //                }
            //            }
            //        }
            //    }
            //    result = Convert.ToInt32(npgsqlParameters[10].Value);
            //    _logger.LogInformation(string.Format("Number of row affected {0} ", result));

            //    _logger.LogDebug("Exiting from UpdateUtvalgListForIntegration");
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in UpdateUtvalgListForIntegration: " + ex.Message);
            //} 
            #endregion
        }



        /// <summary>
        /// Updates the allow double.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="allowDouble">if set to <c>true</c> [allow double].</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UpdateAllowDouble")]
        public IActionResult UpdateAllowDouble(long listId, bool allowDouble)
        {
            _logger.BeginScope("InSide Into UpdateAllowDouble");
            RequestUpdateAllowDouble request = new RequestUpdateAllowDouble()
            {
                AllowDouble = allowDouble,
                ListId = listId
            };
            return Ok(_mediator.Send(request).Result);
            #region Old Code
            //_logger.LogDebug("Inside into UpdateAllowDouble");
            //int result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            //npgsqlParameters[0].Value = list.ListId;

            //npgsqlParameters[1] = new NpgsqlParameter("p_allowdouble", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = list.AllowDouble == true ? 1 : 0;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.updateallowdouble", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (result == 0)
            //    _logger.LogWarning("Oppdatering av AllowDouble feilet for Utvalglistid: " + list.ListId);
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from UpdateWriteprotectUtvalg"); 
            #endregion

        }


        /// <summary>
        /// Updates the is basis.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="isBasis">if set to <c>true</c> [is basis].</param>
        /// <param name="basedOn">The based on.</param>
        [HttpPut]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UpdateIsBasis")]
        public IActionResult UpdateIsBasis(long listId, bool isBasis, int basedOn)
        {
            _logger.BeginScope("InSide Into UpdateIsBasis");
            #region Old Code
            //if (list.BasedOn > 0)
            //    throw new Exception("Lista er basert på en annen liste og kan ikke være basisliste.");
            //try
            //{
            //    //Update Lister
            //    _logger.LogDebug("Inside into UpdateIsBasis");
            //    int result;
            //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //    npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Bigint);
            //    npgsqlParameters[0].Value = list.ListId;

            //    npgsqlParameters[1] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Bigint);
            //    npgsqlParameters[1].Value = list.IsBasis == true ? 1 : 0;

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.updateisbasis", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    if (result == 0)
            //        _logger.LogWarning("Oppdatering av IsBasis feilet for Utvalglistid: " + list.ListId);
            //    //Update Utvalg
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.updateisbasisfromutvalg", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    if (result == 0)
            //        _logger.LogWarning("Oppdatering av IsBasis på utvalg feilet for Utvalglistid: " + list.ListId);
            //}
            //catch
            //{
            //    throw;
            //} 
            #endregion
            RequestUpdateIsBasis request = new RequestUpdateIsBasis()
            {
                IsBasis = isBasis,
                BasedOn = basedOn,
                ListId = listId
            };
            return Ok(_mediator.Send(request).Result);
        }

        // [HttpGet("GetUtvalgListSimpleByKundeNr", Name = nameof(GetUtvalgListSimpleByKundeNr))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtvalgListSimpleByKundeNr>), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListSimpleByKundeNr")]
        public IActionResult GetUtvalgListSimpleByKundeNr(string KundeNummer)
        {
            _logger.BeginScope("InSide Into GetUtvalgListSimpleByKundeNr");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgListSimpleByKundeNr");
            //DataTable utvData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            //npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            ////npgsqlParameters[0].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            //npgsqlParameters[0].Value = KundeNummer;
            ////npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            //////npgsqlParameters[1].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            ////npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistsimplebykundenr", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    utvLColl.Add(CreateListFromRow(row));
            //AddSistOppdatertToLists(utvLColl);
            //return utvLColl;
            #endregion Old Code
            RequestGetUtvalgListSimpleByKundeNr request = new RequestGetUtvalgListSimpleByKundeNr()
            {
                kundenummer = KundeNummer
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///      Returns a list of all campaigns based on a basislist's parent list.
        ///      </summary>
        ///      <param name="listId"></param>
        ///      <returns></returns>
        ///      <remarks></remarks>
        //[HttpGet("GetUtvalgListParentCampaigns", Name = nameof(GetUtvalgListParentCampaigns))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtvalgListParentCampaigns>), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListParentCampaigns")]
        public IActionResult GetUtvalgListParentCampaigns(int listId)
        {
            _logger.BeginScope("InSide Into GetUtvalgListParentCampaigns");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgListParentCampaigns");
            //DataTable listData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //List<CampaignDescription> cdColl = new List<CampaignDescription>();
            //npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer, 50);

            //npgsqlParameters[0].Value = listId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    listData = dbhelper.FillDataTable("kspu_db.getutvalglistparentcampaigns", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow row in listData.Rows)
            //    cdColl.Add(GetCampaignDescriptionFromListDataRow(row, false));
            //_logger.LogInformation("Number of row returned: ", cdColl.Count);
            //_logger.LogDebug("Exiting from GetUtvalgListParentCampaigns");

            //return cdColl;
            #endregion Old Code
            RequestGetUtvalgListParentCampaigns request = new RequestGetUtvalgListParentCampaigns()
            {
                listId = listId
            };
            return Ok(_mediator.Send(request).Result);
        }
        ///// <summary>
        /////      Checks if there are campaigns based on basisutvalgs or basislists that are part of a top level basislist. Used to verify that the top level basislist can be deleted.
        /////      </summary>
        /////      <param name="listId"></param>
        /////      <returns></returns>
        /////      <remarks></remarks>
        //public  bool IsBasisListDeletable(int listId)
        //{
        //    // TODO. Currently verification is done in the GUI, but should ideally be done here by querying the database.
        //    // Dim cmd As New OracleCommand(" SELECT COUNT(*) FROM KSPU_DB.Utvalglist WHERE ... ")
        //    return true;
        //}

        /// <summary>
        ///     ''' Returns a list of all campaigns and disconnected campaigns based on a basislist.
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        //[HttpGet("GetUtvalgListCampaigns", Name = nameof(GetUtvalgListCampaigns))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtvalgListCampaigns>), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListCampaigns")]
        public IActionResult GetUtvalgListCampaigns(int listId)
        {
            _logger.BeginScope("InSide Into GetUtvalgListCampaigns");
            #region oldcode
            //DataTable listData;
            //DataTable utvData2;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //List<CampaignDescription> cdColl = new List<CampaignDescription>();
            //npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters[0].Value = listId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    listData = dbhelper.FillDataTable("kspu_db.getutvalglistcampaignsbybasedon", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in listData.Rows)
            //    cdColl.Add(GetCampaignDescriptionFromListDataRow(row, false));

            //NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];
            //npgsqlParameters1[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters1[0].Value = listId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData2 = dbhelper.FillDataTable("kspu_db.getutvalglistcampaignsbywasbasedon", CommandType.StoredProcedure, npgsqlParameters1);
            //}
            //foreach (DataRow row in utvData2.Rows)
            //    cdColl.Add(GetCampaignDescriptionFromListDataRow(row, true));
            //return cdColl;
            #endregion oldcode
            RequestGetUtvalgListCampaigns request = new RequestGetUtvalgListCampaigns()
            {
                listId = listId
            };
            return Ok(_mediator.Send(request).Result);
        }

        //[HttpGet("SearchUtvalgListWithoutReferences", Name = nameof(SearchUtvalgListWithoutReferences))]
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListWithoutReferences>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListWithoutReferences")]
        public IActionResult SearchUtvalgListWithoutReferences(string Utvalglistname, SearchMethod searchMethod, string customerNumber, bool canHaveEmptyCustomerNumber)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListWithoutReferences");
            #region oldcode
            //Utils utils = new Utils();
            //DataTable utvLData;
            //UtvalgsListCollection utvLColl = new UtvalgsListCollection();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //npgsqlParameters[0] = new NpgsqlParameter("p_Kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = customerNumber;

            //npgsqlParameters[1] = new NpgsqlParameter("p_Utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[1].Value = utils.CreateSearchString(Utvalglistname, searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistwithoutreferences", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow row in utvLData.Rows)
            //{
            //    UtvalgList list = CreateListFromRow(row);
            //    utvLColl.Add(list);
            //}

            //return utvLColl;
            #endregion oldcode
            RequestSearchUtvalgListWithoutReferences request = new RequestSearchUtvalgListWithoutReferences()
            {
                Utvalglistname = Utvalglistname,
                customerNumber = customerNumber,
                searchMethod = searchMethod,
                canHaveEmptyCustomerNumber = canHaveEmptyCustomerNumber
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med navn.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - alle childlister eller evt parentlist
        ///     '''  - alle utvalg som tilhører childlisten
        ///     ''' 
        ///     '''  En liste kan inneholde en liste. Max 2 nivåer med lister. Liste i liste.
        ///     '''  Dvs enten har lista ingen parent eller children, eller den har enten en parent eller children..
        ///     ''' </summary>
        ///     ''' <param name="Utvalglistname"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <returns>UtvalgsListCollection</returns>
        ///     ''' <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgListWithChildren>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgListWithChildren")]
        public IActionResult SearchUtvalgListWithChildren(string Utvalglistname, SearchMethod searchMethod)
        {
            _logger.BeginScope("InSide Into SearchUtvalgListWithChildren");
            #region Old Code
            //Utils utils = new Utils();
            //DataTable utvLData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = Utvalglistname;

            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistwithchildren", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //UtvalgsListCollection utvLColl = GetAllListContent(utvLData, false, true);
            //return utvLColl; 
            #endregion
            RequestSearchUtvalgListWithChildren request = new RequestSearchUtvalgListWithChildren()
            {
                Utvalglistname = Utvalglistname,
                searchMethod = searchMethod
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// GetUtvalgListWithAllReferences
        /// </summary>
        /// <param name="UtvalglistId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtvalgListWithAllReferences), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListWithAllReferences")]
        public IActionResult GetUtvalgListWithAllReferences(int UtvalglistId)
        {
            _logger.BeginScope("InSide Into GetUtvalgListWithAllReferences");
            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //Utils utils = new Utils();
            //DataTable utvLData;
            //// swap if list is basedOn
            //UtvalgList shallowList = (UtvalgList)GetUtvalgListNoChild(UtvalglistId);
            //if (shallowList.BasedOn > 0)
            //{
            //    UtvalgList basisList = GetUtvalgListWithAllReferences(shallowList.BasedOn);
            //    return AddBasedOnValuesToList(basisList, shallowList);
            //}

            //var parentListId = GetParentUtvalgListId(UtvalglistId);
            //if (parentListId > 0)
            //    return GetUtvalgListWithAllReferences(parentListId).GetUtvalgListDescendant(UtvalglistId);

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters[0].Value = UtvalglistId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglist", CommandType.StoredProcedure, npgsqlParameters);
            //}
            ////OracleCommand cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE UtvalglistId = :UtvalglistId ");
            ////AddParameterInteger(cmd, "UtvalglistId", UtvalglistId);
            ////DataTable utvLData = GetDataTable(cmd);
            //if (utvLData.Rows.Count > 1)
            //    throw new Exception("found more than one utvalgList with same utvalglistid!");
            //if (utvLData.Rows.Count == 0)
            //    return null/* TODO Change to default(_) if this is not a reference type */;

            //DataRow row = utvLData.Rows[0];

            //UtvalgList list = CreateListFromRow(row);

            //// tilhøtrende utvalg
            //UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(list.ListId, true).Result;
            //foreach (Utvalg u in uc)
            //    //u.List = list;
            //    u.ListId = Convert.ToString(list.ListId);

            //if (row != null)
            //{
            //    if (list.BasedOn < 0)
            //    {
            //        list.ParentList = GetUtvalgList(utils.GetIntFromRow(row, "ParentUtvalgListId"));
            //        if (!list.ParentList.MemberLists.Contains(list))
            //            list.ParentList.MemberLists.Add(list);
            //    }
            //}
            //else
            //{
            //    DataTable utvData;
            //    // Children lister? - Kun dersom det ikke finnes en parent liste.
            //    npgsqlParameters[0] = new NpgsqlParameter("p_ParentUtvalgListId", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //    npgsqlParameters[0].Value = list.ListId;
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        utvData = dbhelper.FillDataTable("kspu_db.getutvalglistwithallreferences", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    //cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE ParentUtvalgListId = :ParentUtvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
            //    //AddParameterInteger(cmd, "ParentUtvalgListId", list.ListId);

            //    int i = 0;
            //    foreach (DataRow r in utvData.Rows)
            //    {
            //        UtvalgList childList = GetUtvalgList(utils.GetIntFromRow(r, "utvalgListId"), false);
            //        // list.MemberLists.Add(childList)
            //        childList.ParentList = list;
            //        // Children utvalg
            //        uc = _utvalgRepository.SearchUtvalgByUtvalListId(utils.GetIntFromRow(r, "utvalgListId"), true).Result;
            //        foreach (Utvalg u in uc.ToArray())
            //            //u.List = childList;
            //            u.ListId = Convert.ToString(childList.ListId);
            //        i += 1;
            //    }
            //}

            //UtvalgsListCollection dummyColl = new UtvalgsListCollection();
            //dummyColl.Add(list);
            //AddSistOppdatertToLists(dummyColl);

            //return list; 
            #endregion
            RequestGetUtvalgListWithAllReferences request = new RequestGetUtvalgListWithAllReferences()
            {
                UtvalglistId = UtvalglistId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med id.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - alle childlister
        ///     '''  - alle utvalg som tilhører childlisten
        ///     ''' 
        ///     '''  En liste kan inneholde en liste. Max 2 nivåer med lister. Liste i liste. 
        ///     '''  Dvs enten har lista ingen parent eller children, eller den har enten en parent eller children..
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <param name="getParentListMemberUtvalg"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtvalgListWithChildren), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListWithChildren")]
        public IActionResult GetUtvalgListWithChildren(int listId, bool getParentListMemberUtvalg = false)
        {
            _logger.BeginScope("InSide Into GetUtvalgListWithChildren");
            #region Old Code
            //DataTable utvLData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //npgsqlParameters[1] = new NpgsqlParameter("p_utvalgListId", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[1].Value = listId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglistwithchildren", CommandType.StoredProcedure, npgsqlParameters);
            //}
            ////OracleCommand cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE UtvalgListId = :utvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
            ////AddParameterInteger(cmd, "utvalgListId", listId);

            //if (utvLData.Rows.Count != 1)
            //    throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            //DataRow row = utvLData.Rows[0];
            //UtvalgList list = CreateListFromRow(row);

            //// TODO: Refactor code below to use GetAllListContent(..)

            //// swap if list is basedOn
            //if (list.BasedOn > 0)
            //    listId = list.BasedOn;

            //// tilhørende utvalg
            //UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(listId).Result;
            //foreach (Utvalg u in uc)
            //{
            //    //if (u.List != list)
            //    if (Convert.ToInt32(u.ListId) != list.ListId)
            //    {

            //        //if (u.List != list)
            //        //    u.List = list;
            //        u.ListId = list.ListId.ToString();
            //    }
            //}

            //if (row != null)
            //{
            //    if (list.BasedOn < 0)
            //    {
            //        list.ParentList = GetUtvalgList(utils.GetIntFromRow(row, "ParentUtvalgListId"), false, getParentListMemberUtvalg);
            //        if (!list.ParentList.MemberLists.Contains(list))
            //            list.ParentList.MemberLists.Add(list);
            //    }
            //}
            //else
            //{
            //    // Children lister? - Kun dersom det ikke finnes en parent liste.
            //    DataTable utvData;
            //    npgsqlParameters[1] = new NpgsqlParameter("p_ParentUtvalgListId", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //    npgsqlParameters[1].Value = listId;
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        utvData = dbhelper.FillDataTable("kspu_db.GetUtvalgListWithChildren", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    //cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE ParentUtvalgListId = :ParentUtvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
            //    //AddParameterInteger(cmd, "ParentUtvalgListId", listId);

            //    int i = 0;
            //    foreach (DataRow r in utvData.Rows)
            //    {
            //        UtvalgList childList = GetUtvalgList(utils.GetIntFromRow(r, "utvalgListId"), false);

            //        if (childList.BasedOn < 0)
            //        {
            //            list.MemberLists.Add(childList);
            //            childList.ParentList = list;
            //            // Children utvalg
            //            uc = _utvalgRepository.SearchUtvalgByUtvalListId(utils.GetIntFromRow(r, "utvalgListId")).Result;
            //            foreach (Utvalg u in uc)
            //                //  list.MemberLists.Item(i).MemberUtvalgs.Add(u);
            //                i += 1;
            //        }
            //    }
            //}

            //if (list.BasedOn > 0)
            //{
            //    UtvalgList checkIfBasedOnList = (UtvalgList)GetUtvalgListNoChild(list.BasedOn);
            //    list = AddBasedOnValuesToList(list, checkIfBasedOnList);
            //}

            //return list; 
            #endregion

            RequestGetUtvalgListWithChildren request = new RequestGetUtvalgListWithChildren()
            {
                listId = listId,
                getParentListMemberUtvalg = getParentListMemberUtvalg
            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Deletes the campaign list.
        /// </summary>
        /// <param name="BasedOn">The based on.</param>
        /// <param name="ListId">The list identifier.</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("DeleteCampaignList")]
        public IActionResult DeleteCampaignList(int BasedOn, int ListId)
        {
            _logger.BeginScope("InSide Into DeleteCampaignList");
            #region Old Code
            //if (list.BasedOn == 0)
            //    throw new Exception("Kan ikke slette liste, da listen ikke er en kampanje.");
            //int result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[1].Value = list.ListId;
            //// Delete Modification. Slett for hovedliste
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.deletecampaignlist", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //// Delete Liste
            ////using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            ////{
            ////    result = dbhelper.ExecuteNonQuery("kspu_db.DeleteCampaignList", CommandType.StoredProcedure, npgsqlParameters);
            ////}
            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from DeleteCampaignList"); 
            #endregion

            RequestDeleteCampaignList request = new RequestDeleteCampaignList()
            {
                listId = ListId,
                BasedOn = BasedOn
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Metoden sletter utvalgslisten. Man kan velge å slette kun listen eller listen med alle underliggende elementer.
        ///     ''' Dersom sletting med underliggende elementer er metoden rekursiv.
        ///     ''' Sletter man kun listen settes alle nøkler til 'null' i relaterte utvalg og lister.
        ///     ''' Hvis parentlista blir tom som et resultat av slettingen, slettes også denne (med mindre den har kampanjer), og metoden returnerer true. Ellers returneres false.
        ///     ''' </summary>
        ///     ''' <param name="UtvalgListId"></param>
        ///     ''' <param name="withChildren"></param>
        ///     ''' <param name="userName"></param>
        ///     ''' <remarks></remarks>

        [HttpDelete]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("DeleteUtvalgList")]
        public IActionResult DeleteUtvalgList(int UtvalgListId, bool withChildren, string userName)
        {
            _logger.BeginScope("InSide Into DeleteUtvalgList");
            #region Old Code
            //// TODO: Skriv om til å bruke transaksjoner
            //int result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //npgsqlParameters[1] = new NpgsqlParameter("p_UtvalgListId", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[1].Value = UtvalgListId;
            //UtvalgList ul = GetUtvalgListWithChildren(UtvalgListId);
            //if (ul.BasedOn > 0)
            //    throw new Exception("Kan ikke slette kampanjeliste i metoden DeleteUtvalgList, bruk metoden DeleteCampaignList.");

            //// Finner evt parentlistid
            //int ParentUtvalgListId = GetParentUtvalgListId(UtvalgListId);

            //if (withChildren)
            //{
            //    // Vi sletter alle utvalg
            //    if (ul.MemberUtvalgs.Count > 0)
            //    {
            //        //Will merge this code from ramya Code
            //        foreach (Utvalg utvalg in ul.MemberUtvalgs)
            //            utvalgController.DeleteUtvalg(utvalg.UtvalgId, userName);
            //    }
            //    // Vi sletter alle child-lister med deres utvalg, dersom det finnes lister i listen
            //    if (ul.MemberLists.Count > 0)
            //    {
            //        foreach (UtvalgList utvList in ul.MemberLists)
            //            DeleteUtvalgList(utvList.ListId, true, userName);
            //    }

            //    // Delete Modification. Slett for lista

            //    // Delete Modification. Slett for hovedliste
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
            //    }


            //    // Delete Liste
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
            //    }

            //}
            //else
            //{
            //    // Vi fjerner listeid'en i alle utvalg
            //    if (ul.MemberUtvalgs.Count > 0)
            //    {
            //        foreach (Utvalg utvalg in ul.MemberUtvalgs.ToArray())
            //        {
            //            //utvalg.List = null;
            //            utvalg.ListId = null;
            //            // utvalgController.SaveUtvalg(utvalg, userName);

            //        }
            //    }
            //    // Vi setter alle parentlistid i alle child-lister til ingenting
            //    //if (ul.MemberLists.Count > 0)
            //    //{
            //    //    foreach (UtvalgList utvList in ul.MemberLists.ToArray())
            //    //    {
            //    //        utvList.ParentList = null;
            //    //        SaveUtvalgList(utvList);
            //    //    }
            //    //}

            //    // Delete Modification. Slett for hovedliste
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    //cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgListModification WHERE UtvalgListId = :UtvalgListId ");
            //    //AddParameterInteger(cmd, "UtvalgListID", UtvalgListId);
            //    //ExecuteNonQuery(cmd);

            //    // Delete Liste
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    //cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgList WHERE UtvalgListId = :UtvalgListId ");
            //    //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
            //    //ExecuteNonQuery(cmd);
            //}

            //// oppdater evt listeantall
            //if (ParentUtvalgListId > 0)
            //{
            //    UpdateAntallInList(ParentUtvalgListId);

            //    try
            //    {
            //        SaveUtvalgListModifications(ParentUtvalgListId, userName, "DeleteUtvalgList - " + UtvalgListId);
            //        // CommitTransaction(trans);
            //    }
            //    catch (Exception ex)
            //    {

            //        // RollbackTransaction(trans);
            //        throw;
            //    }
            //    // Sletter parent listen hvis den er tom
            //    return CheckAndDeleteUtvalgListIfEmpty(ParentUtvalgListId, userName);
            //}
            //return false; 
            #endregion

            RequestDeleteUtvalgList request = new RequestDeleteUtvalgList()
            {
                userName = userName,
                UtvalgListId = UtvalgListId,
                withChildren = withChildren

            };
            return Ok(_mediator.Send(request).Result);
        }
        /// <summary>
        ///     ''' Metoden kalles etter at man sletter et utvalg.
        ///     ''' Metoden sletter listen utvalget lå i, dersom listen ble tom pga. sletting.
        ///     ''' Inneholder listen andre lister slettes den ikke, men den slettes selv om den ligger som barn av en annen liste.
        ///     ''' </summary>
        ///     ''' <param name="UtvalgListId"></param>
        ///     ''' <param name="userName"></param>
        ///     ''' <remarks></remarks>

        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("CheckAndDeleteUtvalgListIfEmpty")]
        public IActionResult CheckAndDeleteUtvalgListIfEmpty(int UtvalgListId, string userName)
        {
            _logger.BeginScope("InSide Into CheckAndDeleteUtvalgListIfEmpty");
            #region Old Code
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //npgsqlParameters[1] = new NpgsqlParameter("p_UtvalgListId", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[1].Value = UtvalgListId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
            //}
            ////OracleCommand cmd;
            ////cmd = new OracleCommand(" SELECT count(*) FROM KSPU_DB.UtvalgList WHERE ParentUtvalgListId = :UtvalgListId ");
            ////AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
            ////DataTable utvData = GetDataTable(cmd);
            //DataRow row;
            //row = utvData.Rows[0];
            //int count1 = (int)row[0];
            //if (count1 > 0)
            //    return false;

            //// En tom basis listen skal ikke kunne slettes om den har kampanjer knyttet til seg. 
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
            //}

            ////cmd = new OracleCommand(" SELECT count(*) FROM KSPU_DB.UtvalgList WHERE basedon = :UtvalgListId ");
            ////AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
            ////utvData = GetDataTable(cmd);
            //row = utvData.Rows[0];
            //int countK = (int)row[0];
            //if (countK > 0)
            //    return false;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
            //}

            ////cmd = new OracleCommand(" SELECT count(*) FROM KSPU_DB.Utvalg WHERE UtvalgListId = :UtvalgListId ");
            ////AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
            ////utvData = GetDataTable(cmd);
            //row = utvData.Rows[0];
            //int count = (int)row[0];
            //if (count == 0)
            //{
            //    DeleteUtvalgList(UtvalgListId, false, userName);
            //    return true;
            //}
            //return false; 
            #endregion

            RequestCheckAndDeleteUtvalgListIfEmpty request = new RequestCheckAndDeleteUtvalgListIfEmpty()
            {
                userName = userName,
                UtvalgListId = UtvalgListId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Metoden sletter alle utvalgslister som er tomme og deretter de gjenstående tomme liste i liste forekomstene. 
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        [HttpDelete("DeleteEmptyUtvalgLists", Name = nameof(DeleteEmptyUtvalgLists))]
        public void DeleteEmptyUtvalgLists()
        {
            _logger.BeginScope("InSide Into DeleteEmptyUtvalgLists");
            try
            {
                int result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.DeleteEmptyUtvalgLists", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgList WHERE BasedOn=0 AND UtvalgListId IN (SELECT utvalglistid FROM KSPU_DB.utvalglist MINUS " + " SELECT parentutvalglistid FROM KSPU_DB.utvalglist WHERE parentutvalglistid IS NOT NULL MINUS SELECT utvalglistid FROM KSPU_DB.utvalg)");
                // Slett liste i liste.
                //while (result > 0)
                //dbhelper.exc("commit");


                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.DeleteEmptyUtvalgLists", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand("DELETE FROM KSPU_DB.UTVALGLISTMODIFICATION WHERE UTVALGLISTID IN (SELECT UTVALGLISTID FROM KSPU_DB.UTVALGLISTMODIFICATION " + "MINUS SELECT UTVALGLISTID FROM KSPU_DB.UTVALGLIST)");

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw new Exception("exception", exception);
            }
        }


        /// <summary>
        /// Updates the list logo.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UpdateListLogo")]
        public IActionResult UpdateListLogo([FromBody] RequestUpdateListLogo request, string userName)
        {
            request.UserName = userName;
            return Ok(_mediator.Send(request).Result);
        }
        /// <summary>
        /// Gets the nr of double coverage reols for list.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("GetNrOfDoubleCoverageReolsForList")]
        public IActionResult GetNrOfDoubleCoverageReolsForList(int listId)
        {
            _logger.BeginScope("InSide Into GetNrOfDoubleCoverageReolsForList");
            #region Oldcode
            //UtvalgList checkIfBasedOnList = (UtvalgList)GetUtvalgListNoChild(listId);
            //if (checkIfBasedOnList.BasedOn > 0)
            //    listId = checkIfBasedOnList.BasedOn;

            //System.Text.StringBuilder query = new System.Text.StringBuilder();
            //int result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = listId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<int>("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //return System.Convert.ToInt32(result);
            #endregion Oldcode
            RequestGetNrOfDoubleCoverageReolsForList request = new RequestGetNrOfDoubleCoverageReolsForList()
            {
                listId = listId
            };
            return Ok(_mediator.Send(request).Result);
        }

        //[HttpGet("FindUtvalgListsWhithCustomerNumberRestrictions", Name = nameof(FindUtvalgListsWhithCustomerNumberRestrictions))]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseFindUtvalgListsWhithCustomerNumberRestrictions), (int)HttpStatusCode.OK)]
        [Route("FindUtvalgListsWhithCustomerNumberRestrictions")]
        public IActionResult FindUtvalgListsWhithCustomerNumberRestrictions(string listName, string customerNumber)
        {
            _logger.BeginScope("InSide Into FindUtvalgListsWhithCustomerNumberRestrictions");
            #region OldCode
            //DataTable dtUtvalgList;
            //if (customerNumber == null)
            //    throw new Exception("customerNumber can not be null for FindUtvalgListsWhithCustomerNumberRestrictions");
            //UtvalgsListCollection result = new UtvalgsListCollection();

            ////System.Text.StringBuilder query = new System.Text.StringBuilder();
            ////query.AppendLine("select distinct uvl1.*");
            ////query.AppendLine("from KSPU_DB.utvalglist uvl1");
            ////query.AppendLine("where (uvl1.KUNDENUMMER is null or uvl1.KUNDENUMMER = :Kundenummer)");
            ////query.AppendLine(@"and UPPER(uvl1.utvalglistname) like :ListName escape'\'");
            ////query.AppendLine("and not uvl1.UtvalgListId in");
            ////query.AppendLine("  (");
            ////query.AppendLine("  select utv2.utvalglistId");
            ////query.AppendLine("	from KSPU_DB.utvalg utv2");
            ////query.AppendLine("	where utv2.UTVALGLISTID is not null");
            ////query.AppendLine("	and utv2.kundenummer is not null");
            ////query.AppendLine("	and utv2.kundenummer <> :Kundenummer");
            ////query.AppendLine("  )");

            ////OracleCommand cmd = new OracleCommand(query.ToString());
            ////AddParameterString(cmd, "ListName", DAUtvalg.CreateSearchString(listName, SearchMethod.ContainsIgnoreCase), 50);
            ////AddParameterString(cmd, "Kundenummer", customerNumber, 30);
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            //npgsqlParameters[0] = new NpgsqlParameter("p_listname", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = listName;

            //npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = customerNumber;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    dtUtvalgList = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow row in dtUtvalgList.Rows)
            //    result.Add(CreateListFromRow(row));

            //return result;
            #endregion OldCode
            RequestFindUtvalgListsWhithCustomerNumberRestrictions request = new RequestFindUtvalgListsWhithCustomerNumberRestrictions()
            {

            };
            return Ok(_mediator.Send(request).Result);
        }


        // private List<int> _utvalgListIdsWithIllegalCustomerComposition = null;
        //[HttpGet("UtvalgListIdsWithIllegalCustomerComposition", Name = nameof(UtvalgListIdsWithIllegalCustomerComposition))]
        //public List<int> UtvalgListIdsWithIllegalCustomerComposition
        //{
        //    get
        //    {
        //        if (_utvalgListIdsWithIllegalCustomerComposition != null)
        //            return _utvalgListIdsWithIllegalCustomerComposition;

        //        List<int> result = new List<int>();

        //        foreach (int id in GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer())
        //        {
        //            if (!result.Contains(id))
        //                result.Add(id);
        //        }

        //        foreach (int id in GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg())
        //        {
        //            if (!result.Contains(id))
        //                result.Add(id);
        //        }

        //        foreach (int id in GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList())
        //        {
        //            if (!result.Contains(id))
        //                result.Add(id);
        //        }

        //        foreach (int id in GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg())
        //        {
        //            if (!result.Contains(id))
        //                result.Add(id);
        //        }

        //        _utvalgListIdsWithIllegalCustomerComposition = result;

        //        return _utvalgListIdsWithIllegalCustomerComposition;
        //    }
        //}

        //[HttpGet("GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg", Name = nameof(GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg")]
        public IActionResult GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg()
        {
            _logger.BeginScope("InSide Into GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg");
            #region OldCode
            //List<int> result = new List<int>();
            //DataTable dt;
            ////StringBuilder query = new StringBuilder();
            ////query.AppendLine("select distinct uvl1.utvalglistid");
            ////query.AppendLine("from KSPU_DB.utvalglist uvl1");
            ////query.AppendLine("inner join KSPU_DB.utvalglist uvl2");
            ////query.AppendLine("on uvl1.PARENTUTVALGLISTID = uvl2.UTVALGLISTID");
            ////query.AppendLine("inner join KSPU_DB.utvalg utv1");
            ////query.AppendLine("on utv1.UtvalgListId = uvl1.utvalglistid");
            ////query.AppendLine("inner join KSPU_DB.utvalg utv2");
            ////query.AppendLine("on utv2.UtvalgListId = uvl2.utvalglistid");
            ////query.AppendLine("where utv1.Kundenummer is not null");
            ////query.AppendLine("and utv2.Kundenummer is not null");
            ////query.AppendLine("and utv1.Kundenummer <> utv2.Kundenummer");
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    dt = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
            //}


            //foreach (DataRow row in dt.Rows)
            //    result.Add(System.Convert.ToInt32(row["utvalglistid"]));

            //return result;
            RequestGetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg request = new RequestGetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg()
            {

            };
            return Ok(_mediator.Send(request).Result);
            #endregion OldCode
        }

        //[HttpGet("GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList", Name = nameof(GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList")]
        public IActionResult GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList()
        {
            _logger.BeginScope("InSide Into GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList");
            #region OldCode
            //List<int> result = new List<int>();
            //DataTable dt;
            ////StringBuilder query = new StringBuilder();
            ////query.AppendLine("select distinct uvl1.utvalglistid");
            ////query.AppendLine("from KSPU_DB.utvalglist uvl1");
            ////query.AppendLine("inner join KSPU_DB.utvalglist uvl2");
            ////query.AppendLine("on uvl1.PARENTUTVALGLISTID = uvl2.UTVALGLISTID");
            ////query.AppendLine("inner join KSPU_DB.utvalg utv1");
            ////query.AppendLine("on utv1.UtvalgListId = uvl1.utvalglistid");
            ////query.AppendLine("where utv1.Kundenummer is not null");
            ////query.AppendLine("and uvl2.Kundenummer is not null");
            ////query.AppendLine("and utv1.Kundenummer <> uvl2.Kundenummer");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    dt = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
            //}


            //foreach (DataRow row in dt.Rows)
            //    result.Add(System.Convert.ToInt32(row["utvalglistid"]));

            //return result;
            #endregion OldCode
            RequestGetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList request = new RequestGetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList()
            {

            };
            return Ok(_mediator.Send(request).Result);
        }


        //[HttpGet("GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer", Name = nameof(GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer")]
        public IActionResult GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer()
        {
            _logger.BeginScope("InSide Into GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer");
            #region OldCode
            //List<int> result = new List<int>();

            ////StringBuilder query = new StringBuilder();
            ////query.AppendLine("select distinct utv1.UtvalgListId");
            ////query.AppendLine("from KSPU_DB.utvalg utv1, KSPU_DB.utvalg utv2");
            ////query.AppendLine("where utv1.UtvalgListId is not null");
            ////query.AppendLine("and utv1.UtvalgListId = utv2.UtvalgListId");
            ////query.AppendLine("and utv1.UtvalgId <> utv2.UtvalgId");
            ////query.AppendLine("and utv1.Kundenummer is not null");
            ////query.AppendLine("and utv2.Kundenummer is not null");
            ////query.AppendLine("and (utv1.Kundenummer <> utv2.Kundenummer)");


            //DataTable dt;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    dt = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow row in dt.Rows)
            //    result.Add(System.Convert.ToInt32(row["UtvalgListId"]));

            //return result;
            #endregion OldCode
            RequestGetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer request = new RequestGetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer()
            {

            };
            return Ok(_mediator.Send(request).Result);
        }

        //[HttpGet("GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg", Name = nameof(GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg")]
        public IActionResult GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg()
        {
            _logger.BeginScope("InSide Into GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg");
            #region OldCode
            //List<int> result = new List<int>();

            ////StringBuilder query = new StringBuilder();
            ////query.AppendLine("select distinct uvl.UtvalgListid");
            ////query.AppendLine("from KSPU_DB.UtvalgList uvl");
            ////query.AppendLine("inner join KSPU_DB.Utvalg utv");
            ////query.AppendLine("on uvl.UTVALGLISTID = utv.UTVALGlISTID");
            ////query.AppendLine("where uvl.Kundenummer is not null");
            ////query.AppendLine("and utv.Kundenummer is not null");
            ////query.AppendLine("and uvl.Kundenummer <> utv.Kundenummer");


            //DataTable dt;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    dt = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in dt.Rows)
            //    result.Add(System.Convert.ToInt32(row["UtvalgListid"]));

            //return result;
            #endregion OldCode
            RequestGetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg request = new RequestGetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg()
            {

            };
            return Ok(_mediator.Send(request).Result);

        }

        //[HttpGet("IsUtvalgConnectedToList", Name = nameof(IsUtvalgConnectedToList))]
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("IsUtvalgConnectedToList")]
        public IActionResult IsUtvalgConnectedToList(int utvalgId)
        {
            _logger.BeginScope("InSide Into IsUtvalgConnectedToList");
            #region OldCode
            ////StringBuilder query = new StringBuilder();
            ////query.AppendLine("select count(*)");
            ////query.AppendLine("from KSPU_DB.utvalg");
            ////query.AppendLine("where utvalgId = :UtvalgId");
            ////query.AppendLine("and utvalglistId is not null");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
            //}
            ////OracleCommand cmdListCheck = new OracleCommand(query.ToString());
            ////AddParameterInteger(cmdListCheck, "UtvalgId", utvalgId);

            ////return System.Convert.ToInt32(ExecuteScalar(cmdListCheck)) == 1;
            //return System.Convert.ToInt32(result) == 1;
            #endregion OldCode
            RequestIsUtvalgConnectedToList request = new RequestIsUtvalgConnectedToList()
            {
                utvalgId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }

        //[HttpGet("IsUtvalgConnectedToParentList", Name = nameof(IsUtvalgConnectedToParentList))]
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("IsUtvalgConnectedToParentList")]
        public IActionResult IsUtvalgConnectedToParentList(int utvalgId)
        {
            _logger.BeginScope("InSide Into IsUtvalgConnectedToParentList");
            #region OldCode
            //if (!IsUtvalgConnectedToList(utvalgId))
            //    return false;

            ////StringBuilder query = new StringBuilder();
            ////query.AppendLine("select count(*)");
            ////query.AppendLine("from KSPU_DB.utvalgList");
            ////query.AppendLine("where utvalgListId = ");
            ////query.AppendLine("(");
            ////query.AppendLine("  select utvalgListId");
            ////query.AppendLine("  from KSPU_DB.utvalg");
            ////query.AppendLine("  where utvalgId = :UtvalgId");
            ////query.AppendLine(")");
            ////query.AppendLine("and parentUtvalgListId is not null");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //return System.Convert.ToInt32(result) == 1;
            #endregion OldCode
            RequestIsUtvalgConnectedToParentList request = new RequestIsUtvalgConnectedToParentList()
            {
                listId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }

        //[HttpGet("GetRootUtvalgListWithAllReferences", Name = nameof(GetRootUtvalgListWithAllReferences))]
        //public UtvalgList GetRootUtvalgListWithAllReferences(int utvalgId)
        //{
        //    if (IsUtvalgConnectedToParentList(utvalgId))
        //        return GetConnectedParentListWithAllReferences(utvalgId);
        //    if (IsUtvalgConnectedToList(utvalgId))
        //        return GetConnectedListWithAllReferences(utvalgId);

        //    return null/* TODO Change to default(_) if this is not a reference type */;
        //}

        //[HttpGet("HasListDemSegUtvalgDescendant", Name = nameof(HasListDemSegUtvalgDescendant))]
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("HasListDemSegUtvalgDescendant")]
        public IActionResult HasListDemSegUtvalgDescendant(int utvalgListId)
        {
            _logger.BeginScope("InSide Into HasListDemSegUtvalgDescendant");
            #region OldCode
            //UtvalgList checkIfBasedOnList = (UtvalgList)GetUtvalgListNoChild(utvalgListId);
            //if (checkIfBasedOnList.BasedOn > 0)
            //    utvalgListId = checkIfBasedOnList.BasedOn;

            ////StringBuilder query = new StringBuilder();
            ////query.AppendLine("select");
            ////query.AppendLine("(select count(distinct u.utvalgId)");
            ////query.AppendLine("from kspu_db.utvalg u");
            ////query.AppendLine("inner join kspu_db.utvalgCriteria c");
            ////query.AppendLine("on u.utvalgId = c.utvalgId");
            ////query.AppendLine("where u.utvalgListId = :ListId");
            ////query.AppendLine("and c.CriteriaType in (2, 12))");
            ////query.AppendLine("+");
            ////query.AppendLine("(select count(distinct u.utvalgId)");
            ////query.AppendLine("from kspu_db.utvalg u");
            ////query.AppendLine("inner join kspu_db.utvalgList ul");
            ////query.AppendLine("on ul.UtvalgListId = u.UtvalgListId");
            ////query.AppendLine("inner join kspu_db.utvalgCriteria c");
            ////query.AppendLine("on u.utvalgId = c.utvalgId");
            ////query.AppendLine("where ul.ParentUtvalgListId = :ListId");
            ////query.AppendLine("and c.CriteriaType in (2, 12))");
            ////query.AppendLine("from dual");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgListId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.haslistdemsegutvalgdescendant", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //return System.Convert.ToInt32((result)) > 0;
            #endregion OldCode
            RequestHasListDemSegUtvalgDescendant request = new RequestHasListDemSegUtvalgDescendant()
            {
                listId = utvalgListId
            };
            return Ok(_mediator.Send(request).Result);

        }

        [HttpGet("FindLastModifiedDateForList", Name = nameof(FindLastModifiedDateForList))]
        public DateTime FindLastModifiedDateForList(int utvalgListId)
        {
            _logger.BeginScope("InSide Into FindLastModifiedDateForList");
            // Supportsak # 621854 PUMA, utskrift av utvalgsliste viser ikke endring i forhandlerpåtrykk
            // Deactivated original code
            // Dim cmd As New OracleCommand("select modificationdate from KSPU_DB.utvalglistmodification where utvalglistid = :utvalgListId order by modificationdate desc")
            // AddParameterInteger(cmd, "utvalgListId", utvalgListId)
            // Dim obj As Object = ExecuteScalar(cmd)

            // If obj Is DBNull.Value Then Return DateTime.MinValue
            // Return obj

            // Supportsak # 621854 PUMA, utskrift av utvalgsliste viser ikke endring i forhandlerpåtrykk - må ha siste dato på listen og hele dens innhold

            // Find last date for all changes in this list including all its childlist
            object objL;
            object objU;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = utvalgListId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                //objL = dbhelper.ExecuteScalar<DateTime>("kspu_db.findlastmodifieddateforlistfromutvalglist", CommandType.StoredProcedure, npgsqlParameters);
                objL = dbhelper.ExecuteScalar<DateTime>("kspu_db.findlastmodifieddateforlistfromutvalglist", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmdL = new OracleCommand("select * from KSPU_DB.utvalglistmodification where utvalglistid in( select utvalglistId from KSPU_DB.utvalglist where utvalglistid in ( select utvalglistid from KSPU_DB.utvalglist where parentutvalglistid = :utvalgListId) or utvalglistid = :utvalgListId) order by modificationdate desc");
            //AddParameterInteger(cmdL, "utvalgListId", utvalgListId);


            // Find last date for all utvalg in this list and all its childlist
            //OracleCommand cmdU = new OracleCommand("select * from KSPU_DB.utvalgmodification where utvalgid in (select utvalgid from KSPU_DB.utvalg where utvalglistid in ( select utvalglistId from KSPU_DB.utvalglist where utvalglistid in ( select utvalglistid from KSPU_DB.utvalglist where parentutvalglistid = :utvalgListId) or utvalglistid = :utvalgListId)) order by modificationdate desc");
            //AddParameterInteger(cmdU, "utvalgListId", utvalgListId);
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                objU = dbhelper.ExecuteScalar<DateTime>("kspu_db.findlastmodifieddateforlistfromutvalglist", CommandType.StoredProcedure, npgsqlParameters);
            }


            DateTime sistListOppdatert = new DateTime(1, 1, 1);
            try
            {
                if (objL != DBNull.Value)
                {
                    if (sistListOppdatert.CompareTo(objL) > 0)
                        sistListOppdatert = (DateTime)objL;
                }

                if (objU != DBNull.Value)
                {
                    if (sistListOppdatert.CompareTo(objU) > 0)
                        sistListOppdatert = (DateTime)objU;
                }
            }
            catch
            {
                return sistListOppdatert;
            }
            return sistListOppdatert;
        }


        /// <summary>
        /// Adds the utvalgs to new list.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseAddUtvalgsToNewList), (int)HttpStatusCode.OK)]
        [Route("AddUtvalgsToNewList")]
        public IActionResult AddUtvalgsToNewList([FromBody] RequestAddUtvalgsToNewList request, string userName)
        {
            _logger.BeginScope("InSide Into AddUtvalgsToNewList");
            request.UserName = userName;
            return Ok(_mediator.Send(request).Result);
            #region Old Code
            //_logger.LogDebug("Inside into UpdateAllowDouble");
            //int result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            //npgsqlParameters[0].Value = list.ListId;

            //npgsqlParameters[1] = new NpgsqlParameter("p_allowdouble", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = list.AllowDouble == true ? 1 : 0;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.updateallowdouble", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (result == 0)
            //    _logger.LogWarning("Oppdatering av AllowDouble feilet for Utvalglistid: " + list.ListId);
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from UpdateWriteprotectUtvalg"); 
            #endregion

        }

        /// <summary>
        /// Adds the utvalgs to existing list.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("AddUtvalgsToExistingList")]
        public IActionResult AddUtvalgsToExistingList([FromBody] RequestAddUtvalgsToExistingList request, string userName)
        {
            _logger.BeginScope("InSide Into AddUtvalgsToExistingList");
            request.userName = userName;
            return Ok(_mediator.Send(request).Result);
            #region Old Code
            //_logger.LogDebug("Inside into UpdateAllowDouble");
            //int result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            //npgsqlParameters[0].Value = list.ListId;

            //npgsqlParameters[1] = new NpgsqlParameter("p_allowdouble", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = list.AllowDouble == true ? 1 : 0;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.updateallowdouble", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (result == 0)
            //    _logger.LogWarning("Oppdatering av AllowDouble feilet for Utvalglistid: " + list.ListId);
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from UpdateWriteprotectUtvalg"); 
            #endregion

        }

        /// <summary>
        ///     ''' Metoden kalles og oppdaterer antallsopplysingen som lagres på en liste når:
        ///     ''' - Et utvalg legges til liste
        ///     ''' - Et utvalg i lista endres
        ///     ''' - Et utvalg fjernes fra lista
        ///     ''' - Et utvalg slettes
        ///     ''' - Ei liste legges til lista
        ///     ''' - Ei liste i lista endres
        ///     ''' - Ei liste fjernes fra lista
        ///     ''' - Ei liste i lista slettes
        ///     ''' </summary>
        ///     ''' <param name="UtvalgListId"></param>
        ///     ''' <remarks></remarks>
        [HttpPut("UpdateAntallInList", Name = nameof(UpdateAntallInList))]
        public void UpdateAntallInList(int UtvalgListId)
        {
            _logger.BeginScope("InSide Into UpdateAntallInList");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid1", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
                npgsqlParameters[0].Value = UtvalgListId;

                npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistid2", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
                npgsqlParameters[1].Value = UtvalgListId;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.updateantallinlist", CommandType.StoredProcedure, npgsqlParameters);
                }

                //OracleCommand cmd = new OracleCommand();
                //string sql = " select sum(antall) from KSPU_DB.utvalg where utvalglistid in (select utvalglistid from KSPU_DB.utvalglist where parentutvalglistid = :UtvalgListId1 or utvalglistid = :UtvalgListId2)";
                //cmd = new OracleCommand(sql);
                //AddParameterInteger(cmd, "UtvalgListId1", UtvalgListId);
                //AddParameterInteger(cmd, "UtvalgListId2", UtvalgListId);

                DataRow row;
                int sumUtvalg = 0;
                if (utvData.Rows.Count > 0)
                {
                    row = utvData.Rows[0];
                    if (!Convert.IsDBNull(row.ItemArray[0]))
                        sumUtvalg = Convert.ToInt32(row.ItemArray[0]);
                }

                int totalsum = sumUtvalg;

                // Oppdater lista med nytt antall
                int result;
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
                npgsqlParameters[0].Value = UtvalgListId;

                npgsqlParameters[1] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
                npgsqlParameters[1].Value = totalsum;
                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateantallinlistforantall", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand(" UPDATE KSPU_DB.UtvalgList SET Antall = :Antall WHERE UtvalgListID = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //AddParameterInteger(cmd, "Antall", totalsum);
                //ExecuteNonQuery(cmd);

                // Oppdater arvet liste med nytt antall
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
                npgsqlParameters[0].Value = UtvalgListId;

                npgsqlParameters[1] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
                npgsqlParameters[1].Value = totalsum;
                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateantallinlistforantallforbasedon", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand(" UPDATE KSPU_DB.UtvalgList SET Antall = :Antall WHERE BasedOn = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //AddParameterInteger(cmd, "Antall", totalsum);
                //ExecuteNonQuery(cmd);

                // Oppdatering av evt parent list etter endring av childlist
                // Finn parent list
                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];
                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
                npgsqlParameters1[0].Value = UtvalgListId;
                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getparentutvalglistid", CommandType.StoredProcedure, npgsqlParameters1);
                }
                //cmd = new OracleCommand(" SELECT ParentUtvalgListId FROM KSPU_DB.UtvalgList WHERE UtvalgListId = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //utvData = GetDataTable(cmd);
                int ParentUtvalgListId = 0;
                if (utvData.Rows.Count > 0)
                {
                    row = utvData.Rows[0];
                    if (!Convert.IsDBNull(row.ItemArray[0]))
                        ParentUtvalgListId = Convert.ToInt32(row.ItemArray[0]);
                }
                // Oppdater liste
                if (ParentUtvalgListId > 0)
                    UpdateAntallInList(ParentUtvalgListId);
            }
            catch (Exception ex)
            {
                throw new Exception("Fikk ikke oppdatert antall på lister" + ex.Message + "for utvalg med Id=" + UtvalgListId);

            }
        }

        [HttpPut("disconnectlist", Name = nameof(DisconnectList))]
        [ProducesResponseType(typeof(ResponseDisconnectList), (int)HttpStatusCode.OK)]
        public IActionResult DisconnectList([FromBody] RequestDisconnectList list, string userName)
        {
            _logger.BeginScope("InSide Into DisconnectList");
            list.UserName = userName;
            return Ok(_mediator.Send(list).Result);
        }


        /// <summary>
        /// Changes the parent list of list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        [HttpPost("changeParentListOfList", Name = nameof(ChangeParentListOfList))]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public IActionResult ChangeParentListOfList([FromBody] RequestChangeParentListOfList list, string userName)
        {
            _logger.BeginScope("InSide Into ChangeParentListOfList");
            list.UserName = userName;
            return Ok(_mediator.Send(list).Result);
        }

        [HttpPost("createNewParentForList", Name = nameof(CreateNewParentForList))]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public IActionResult CreateNewParentForList([FromBody] RequestCreateNewParentForList list, string userName)
        {
            _logger.BeginScope("InSide Into CreateNewParentForList");
            list.UserName = userName;
            return Ok(_mediator.Send(list).Result);
        }

        [HttpPost("CreateCopyOfUtvalgList", Name = nameof(CreateCopyOfUtvalgList))]
        [ProducesResponseType(typeof(ResponseCreateCopyOfUtalgList), (int)HttpStatusCode.OK)]
        public IActionResult CreateCopyOfUtvalgList([FromBody] RequestCreateCopyOfList list, string userName)
        {
            _logger.BeginScope("InSide Into CreateNewParentForList");
            list.userName = userName;
            return Ok(_mediator.Send(list).Result);
        }

        [HttpPut("UpdateUtvalgListDistributionData", Name = nameof(UpdateUtvalgListDistributionData))]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult UpdateUtvalgListDistributionData([FromBody] RequestSaveUtvalgListDistributionData list, string userName)
        {
            _logger.BeginScope("InSide Into UpdateUtvalgListDistributionData");
            list.userName = userName;
            return Ok(_mediator.Send(list).Result);
        }

        #endregion
        #region Private Methods
        /// <summary>
        ///     ''' Get Modifications for list
        ///     ''' </summary>
        ///     ''' <param name="list"></param>
        ///     ''' <remarks></remarks>
        private void GetUtvalgListModifications(UtvalgList list)
        {
            _logger.LogDebug("Inside into GetUtvalgListModifications");
            DataTable utvData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            UtvalgModification utvalgModification = new UtvalgModification();


            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = list.ListId;
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                utvData = dbhelper.FillDataTable("kspu_db.getutvalglistmodifications", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (DataRow dataRow in utvData.Rows)
            {
                utvalgModification.UserId = utils.GetStringFromRow(dataRow, "r_userid");
                utvalgModification.ModificationId = utils.GetIntFromRow(dataRow, "r_utvalglistmodificationid");
                utvalgModification.ModificationTime = utils.GetTimestampFromRow(dataRow, "r_modificationdate");
                list.Modifications.Add(utvalgModification);
            }
            _logger.LogInformation("Number of row returned: ", utvalgModification);
            _logger.LogDebug("Exiting from GetUtvalgListModifications");

        }


        #endregion
    }
}