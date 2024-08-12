using DataAccessAPI;
using DataAccessAPI.Controllers;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.ECPumaHelper
{
    public class MappingUtvalgsdetaljer
    {
        #region Variables
        private readonly ILogger<MappingUtvalgsdetaljer> _logger;
        private readonly UtvalgController utvalgController1;
        private readonly UtvalgListController utvalgListController;
        Utils utils = new Utils();
       // private readonly KapasitetController kapasitetController;
        private readonly IUtvalgRepository _utvalgRepository;
        private readonly IUtvalgListRepository _utvalgListRepository;

        private readonly ConfigController configController;
        private readonly IKapasitetRepository _kapasitetRepository;
        // private readonly MappingAntallsopplysninger mappingAntall;
        #endregion

        #region Constructors        

        
        public MappingUtvalgsdetaljer(ILogger<MappingUtvalgsdetaljer> logger, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList, ILogger<KapasitetController> loggerkapasitet,
            IUtvalgRepository utvalgRepository, IKapasitetRepository kapasitetRepository, IUtvalgListRepository utvalgListRepository)
        {
            _logger = logger;
            _utvalgRepository = utvalgRepository;
            _kapasitetRepository = kapasitetRepository;
            _utvalgListRepository = utvalgListRepository;

            utvalgListController = new UtvalgListController(loggerUtvalgList, loggerConfig, loggerUtvalg, loggerreol,_utvalgRepository);
            utvalgController1 = new UtvalgController(loggerUtvalg, loggerConfig, loggerreol);
          
           // kapasitetController = new KapasitetController(loggerkapasitet,null,null);
            configController = new ConfigController(loggerConfig);

        }
        #endregion

        #region Public Methods

        /// <summary>
        ///     ''' Meoden returnerer en UtvalgAndUtvalgListCollections basert på en gitt id og type
        ///     ''' Det sjekkes om påkrevde paarametre er innsendt
        ///     ''' </summary>
        ///     ''' <param name="ordreStatusService"></param>
        ///     ''' <returns>UtvalgAndUtvalgListCollections</returns>
        ///     ''' <remarks></remarks>
        public UtvalgAndUtvalgListCollections GetCollectionsFromOrdreStatus(OrdreStatusService ordreStatusService)
        {
            _logger.LogDebug("Inside into GetCollectionsFromOrdreStatus");
            UtvalgAndUtvalgListCollections utvalgCollections = new UtvalgAndUtvalgListCollections();
            // kast exception om parametre mangler
            if (ordreStatusService.SistEndretAv == null/* TODO Change to default(_) if this is not a reference type */ )
                throw new ArgumentNullException("Parameter 'ENDRET_AV' er ikke angitt med noen verdi");
            if (ordreStatusService.OEBSRef == null)
                throw new ArgumentNullException("Parameter 'OEBS_REF' er ikke angitt med noen verdi");

            // basert på Ordreinfo (IKKE benyttet da id/type er påkrevd)
            // ulColl = MappingUtvalgsdetaljer.getCollectionFromOEBSRef(ordreStatus.OEBSRef, ordreStatus.OEBSType, ordreStatus.ReturnerFordeling)
            utvalgCollections = GetCollectionsFromIdType(Convert.ToInt32(ordreStatusService.UtvalgId), ordreStatusService.Type, ordreStatusService.ReturnerFordeling);
            _logger.LogInformation("Data Returned: " + utvalgCollections);
            _logger.LogDebug("Exiting From GetCollectionsFromOrdreStatus");
            return utvalgCollections;
        }

        public UtvalgAndUtvalgListCollections GetCollectionsFromIdType(int id, UtvalgsTypeKode type, bool returfordeling)
        {
            // oppretter collections basert på type og id
            if (type == UtvalgsTypeKode.Utvalg)
                return getCollectionFromUtvalgId(id, returfordeling);
            else
                return getCollectionFromListId(id, returfordeling);
        }

        public UtvalgAndUtvalgListCollections getCollectionFromUtvalgId(int id, bool getReolInfo)
        {
            _logger.LogDebug("Inside into getCollectionFromUtvalgId");
            UtvalgCollection utvalgCollection = _utvalgRepository.SearchUtvalgByUtvalgId(id, getReolInfo).Result;
            UtvalgAndUtvalgListCollections ulColl = new UtvalgAndUtvalgListCollections();
            ulColl.Utvalgs = utvalgCollection;
            _logger.LogInformation("Data Returned: " + ulColl.Utvalgs);
            _logger.LogDebug("Exiting From getCollectionFromUtvalgId");
            return ulColl;
        }

        public UtvalgAndUtvalgListCollections getCollectionFromListId(int id, bool getReolInfo)
        {
            _logger.LogDebug("Inside into getCollectionFromListId");
            UtvalgAndUtvalgListCollections ulColl = new UtvalgAndUtvalgListCollections();
            UtvalgsListCollection utvalgListCollection = _utvalgListRepository.SearchUtvalgListWithChildrenById(id, getReolInfo).Result;
            ulColl.UtvalgLists = utvalgListCollection;
            _logger.LogInformation("Data Returned: " + ulColl.UtvalgLists);
            _logger.LogDebug("Exiting From getCollectionFromListId");
            return ulColl;
        }

        public UtvalgAndUtvalgListCollections getCollectionFromOEBSRef(string OEBSRef, OEBSTypeKode OEBSType, bool getReolInfo)
        {
            _logger.LogDebug("Inside into getCollectionFromOEBSRef");
            string OrdreType = Utils.EntityMapping(OEBSType).ToString();
            UtvalgCollection utvalgColl = _utvalgRepository.SearchUtvalgByOrdreReferanse(OEBSRef, OrdreType, SearchMethod.EqualsIgnoreCase, getReolInfo).Result;
            UtvalgsListCollection utvalgListColl = (UtvalgsListCollection)utvalgListController.SearchUtvalgListSimpleByOrdreReferanse(OEBSRef, OrdreType, SearchMethod.EqualsIgnoreCase);

            UtvalgAndUtvalgListCollections ulColl = new UtvalgAndUtvalgListCollections();
            ulColl.Utvalgs = utvalgColl;
            ulColl.UtvalgLists = utvalgListColl;
            _logger.LogInformation("Data Returned UtvalgList: " + ulColl.UtvalgLists + " Utvalg: " + ulColl.Utvalgs );
            _logger.LogDebug("Exiting From getCollectionFromOEBSRef");
            return ulColl;
        }

        public void DBUpdateUtvalgAndListCollection(UtvalgAndUtvalgListCollections ulColl, OrdreStatusService ordreStatusService)
        {
            // Due to changes, its now alway only 1 utvalg or list in UtvalgAndUtvalgListCollections !
            _logger.LogDebug("Inside into DBUpdateUtvalgAndListCollection");
            // update utvalgCollection
            if (ulColl.Utvalgs.Count > 0)
            {
                Utvalg utvalg = ulColl.Utvalgs[0];
                if ((utvalg.OrdreType == OrdreType.Null && (utvalg.OrdreReferanse == null || utvalg.OrdreReferanse.Length == 0)))
                {
                    string mType = "HH";
                    if ((utvalg.Receivers.HasOnlyReceiverType(ReceiverType.Businesses)))
                        mType = "VH";
                    try
                    {
                        List<long> ruteIds = _utvalgRepository.GetUtvalgReolIDs(utvalg.BasedOn > 0? utvalg.BasedOn: utvalg.UtvalgId).Result;
                        switch (utvalg.DistributionType)
                        {
                            case (DistributionType)Distribusjonstype.B:
                                {
                                    _kapasitetRepository.SubtractRestkapasitetAbsoluttDag(ruteIds, utvalg.Weight, utvalg.DistributionDate, mType, utvalg.Thickness).Wait();
                                    break;
                                }

                            case (DistributionType)Distribusjonstype.S:
                                {
                                    _kapasitetRepository.SubtractRestkapasitetSperrefrist(ruteIds, utvalg.Weight, utvalg.DistributionDate, mType, utvalg.Thickness).Wait();
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error subtracting restkapasitet for utvalg " + utvalg.UtvalgId + ": " + ex.ToString());
                    }
                }
                SetNewValuesOnUtvalg(utvalg, ordreStatusService);
                _utvalgRepository.UpdateUtvalgForIntegration(utvalg, ordreStatusService.SistEndretAv).Wait();
                _logger.LogDebug("Exiting From DBUpdateUtvalgAndListCollection");
            }
            else if (ulColl.UtvalgLists.Count > 0)
            {
                // update utvalgCollection (lister og evt tilhørende utvalg)
                UtvalgList ul = ulColl.UtvalgLists[0];
                if ((ul.OrdreType == OrdreType.Null && (ul.OrdreReferanse == null || ul.OrdreReferanse.Length == 0)))
                {
                    UtvalgReceiverList receivers =(UtvalgReceiverList)_utvalgListRepository.GetUtvalgListReceivers(ul.ListId).Result;
                    string mType = "HH";
                    if ((receivers.HasOnlyReceiverType(ReceiverType.Businesses)))
                        mType = "VH";
                    try
                    {
                        List<long> ruteIds = (List<long>)_utvalgListRepository.GetUtvalgListReolIDs(ul.BasedOn > 0? ul.BasedOn: ul.ListId).Result;
                        switch (ul.DistributionType)
                        {
                            case (DistributionType)Distribusjonstype.B:
                                {
                                    _kapasitetRepository.SubtractRestkapasitetAbsoluttDag(ruteIds, ul.Weight, ul.DistributionDate, mType, ul.Thickness).Wait();
                                    break;
                                }

                            case (DistributionType)Distribusjonstype.S:
                                {
                                    _kapasitetRepository.SubtractRestkapasitetSperrefrist(ruteIds, ul.Weight, ul.DistributionDate, mType, ul.Thickness).Wait();
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error subtracting restkapasitet for utvalglist " + ul.ListId + ": " + ex.ToString());
                    }
                }
                SetNewValuesOnList(ul, ordreStatusService);
                _utvalgListRepository.UpdateUtvalgListForIntegration(ul, ordreStatusService.SistEndretAv).Wait();
                _logger.LogDebug("Exiting From DBUpdateUtvalgAndListCollection");
            }
        }

        public bool ExistsOrdreReferanseInKSPU(UtvalgAndUtvalgListCollections ulColl)
        {
            // alle utvalg og lister skal ha samme OrdreRef/ordreType, om den finnes        
            if ((ulColl.Utvalgs.Count > 0) && (ulColl.Utvalgs[0].OrdreReferanse != null))
                // utvalg, sjekker kun den første
                return true;
            else if ((ulColl.UtvalgLists.Count > 0) && (ulColl.UtvalgLists[0].OrdreReferanse != null))
                // lister, sjekker kun den første
                return true;
            else
                return false;
        }

        public bool IsOrdreReferanseValid(UtvalgAndUtvalgListCollections ulColl, OrdreStatusService ordreStatusService)
        {
            // alle utvalg og lister skal ha samme OrdreRef/ordreType. 
            OrdreType innsentOrdreType = Utils.EntityMapping(ordreStatusService.OEBSType);
            if ((ulColl.Utvalgs.Count > 0))
            {
                // utvalg, sjekker kun den første
                Utvalg u = ulColl.Utvalgs[0];
                if (u.OrdreReferanse == ordreStatusService.OEBSRef && u.OrdreType.Equals(innsentOrdreType))
                    return true;
                else
                    return false;
            }
            else if ((ulColl.UtvalgLists.Count > 0))
            {
                // lister, sjekker kun den første
                UtvalgList ul = ulColl.UtvalgLists[0];
                if (ul.OrdreReferanse == ordreStatusService.OEBSRef && ul.OrdreType.Equals(innsentOrdreType))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }


        public Utvalgsfordeling MapKSPUCollectionToWSUtvalgsfordelingByIdType(UtvalgAndUtvalgListCollections ulColl, int id, UtvalgsTypeKode type)
        {
            _logger.LogDebug("Inside into MapKSPUCollectionToWSUtvalgsfordelingByIdType");
            // create temporary OrdreStatus for not to duplicate methods
            OrdreStatusService ordreStatus = new OrdreStatusService();
            if ((ulColl.Utvalgs.Count > 0))
            {
                Utvalg utvalg = ulColl.Utvalgs[0];
                ordreStatus.UtvalgId = id;
                ordreStatus.Type = type;
                ordreStatus.OEBSRef = utvalg.OrdreReferanse;
                ordreStatus.OEBSType = Utils.EntityMapping(utvalg.OrdreType);
            }
            else if ((ulColl.UtvalgLists.Count > 0))
            {
                UtvalgList utvalgList = ulColl.UtvalgLists[0];
                ordreStatus.UtvalgId = id;
                ordreStatus.Type = type;
                ordreStatus.OEBSRef = utvalgList.OrdreReferanse;
                ordreStatus.OEBSType = Utils.EntityMapping(utvalgList.OrdreType);
            }
            
            _logger.LogDebug("Exiting From MapKSPUCollectionToWSUtvalgsfordelingByIdType");
            return MapKSPUCollectionToWSUtvalgsfordeling(ulColl, ordreStatus);
        }

        public Utvalgsfordeling MapKSPUCollectionToWSUtvalgsfordeling(UtvalgAndUtvalgListCollections ulColl, OrdreStatusService ordreStatusService)
        {
            _logger.LogDebug("Inside into MapKSPUCollectionToWSUtvalgsfordeling");
            Utvalgsfordeling utvFordeling = new Utvalgsfordeling();

            if ((ulColl.Utvalgs.Count > 0))
            {
                // utvalg, sjekker kun den første
                Utvalg utvalg = ulColl.Utvalgs[0];
                utvFordeling.UtvalgId = ordreStatusService.UtvalgId;
                utvFordeling.UtvalgsRef = utvalg.Name;
                utvFordeling.OEBSRef = ordreStatusService.OEBSRef;
                utvFordeling.OEBSType = ordreStatusService.OEBSType;
                utvFordeling.Type = UtvalgsTypeKode.Utvalg;

                if (utvalg.Modifications != null)
                {
                    DateTime sistOppdatert = new DateTime(1, 1, 1);
                    foreach (UtvalgModification m in utvalg.Modifications)
                    {
                        if (m.ModificationTime.CompareTo(sistOppdatert) > 0)
                            sistOppdatert = m.ModificationTime;
                    }
                    utvFordeling.SistOppdatert = sistOppdatert;
                }

                List<Utvalgsdetaljer> utvDetList = new List<Utvalgsdetaljer>();
                utvDetList.Add(CreateUtvalgsdetaljerForUtvalg(utvalg));
                utvFordeling.Utvalg = utvDetList;
            }
            else if ((ulColl.UtvalgLists.Count > 0))
            {
                // lister, sjekker kun den første for det finnes kun 1
                UtvalgList utvalgList = ulColl.UtvalgLists[0];
                utvFordeling.UtvalgId = ordreStatusService.UtvalgId;
                utvFordeling.UtvalgsRef = utvalgList.Name;
                utvFordeling.OEBSRef = ordreStatusService.OEBSRef;
                utvFordeling.OEBSType = ordreStatusService.OEBSType;
                utvFordeling.Type = UtvalgsTypeKode.Liste;
                utvFordeling.SistOppdatert = utvalgList.SistOppdatert;
                // legger til <ToppListeLogo>  på lister. Logo er Nothing om den ikke er satt.
                utvFordeling.Logo = utvalgList.Logo;

                List<Utvalgsdetaljer> utvDetList = new List<Utvalgsdetaljer>();
                foreach (Utvalg utv in utvalgList.MemberUtvalgs)
                    utvDetList.Add(CreateUtvalgsdetaljerForUtvalg(utv));
                if (utvalgList.MemberLists.Count > 0)
                {
                    // Legg til info for hver liste og dets utvalg
                    foreach (UtvalgList childUl in utvalgList.MemberLists)
                    {
                        utvDetList.Add(CreateUtvalgsdetaljerForUtvalgList(childUl));
                        foreach (Utvalg utv in childUl.MemberUtvalgs)
                            utvDetList.Add(CreateUtvalgsdetaljerForUtvalg(utv));
                    }
                }

                utvFordeling.Utvalg = utvDetList;
            }

            // sjekk om alle ruter innenfor et team er med i utvalget

            CheckIfAllRuterInTeam(utvFordeling);

            _logger.LogInformation("Data Returned: " + utvFordeling);
            _logger.LogDebug("Exiting From MapKSPUCollectionToWSUtvalgsfordeling");
            return utvFordeling;
        }

        #endregion


        #region Private Methods

        private void SetNewValuesOnUtvalg(Utvalg utvalg, OrdreStatusService ordreStatusService)
        {
            utvalg.OrdreStatus = Utils.EntityMapping(ordreStatusService.Status);

            if (ordreStatusService.Innleveringdato != null/* TODO Change to default(_) if this is not a reference type */ )
                utvalg.InnleveringsDato = Convert.ToDateTime(ordreStatusService.Innleveringdato);

            if (ordreStatusService.Avtalenummer != null/* TODO Change to default(_) if this is not a reference type */ )
                utvalg.Avtalenummer = ordreStatusService.Avtalenummer;

            if (ordreStatusService.Status == OrdreTilbudStatusKode.Kansellert)
            {
                utvalg.OrdreReferanse = null;
                utvalg.OrdreType = OrdreType.Null;
                utvalg.Avtalenummer = 0;
            }
            else if (ordreStatusService.OEBSRef != null)
            {
                utvalg.OrdreReferanse = ordreStatusService.OEBSRef;
                utvalg.OrdreType = Utils.EntityMapping(ordreStatusService.OEBSType);

                // 20110926 LHK: Posten ønsker at utvalg automatisk skal frikobles fra basisutvalg i det ordre opprettes (i det PUMA mottar ordrestatusmelding med OEBS type = "O"). 
                if (utvalg.BasedOn > 0 && Convert.ToInt32(utvalg.OrdreType) == Convert.ToInt32(OEBSTypeKode.Ordre))
                    // KSPU.BusinessLayer.BasisUtvalgManager.DisconnectUtvalg(u, ordreStatus.SistEndretAv) -> Gir feil hos Posten mangler mottakere.. skipper forretningslaget - laget ny frikobling i Databaselaget
                    // Integration fails in check against utv.TotalAntall but needs to save utvalg if disconnected 
                    // TO MINIMIZE risk manual save of disconnected utvalg used insted for IntegrationSaveUtvalg version - UpdateDisconnectUtvalgForIntegration - NO CHANGES IN SAVEUTVALG.
                    _utvalgRepository.UpdateDisconnectUtvalgForIntegration(utvalg, ordreStatusService.SistEndretAv).Wait();
            }

            if (!ordreStatusService.DistributionDate.Equals(DateTime.MinValue))
                utvalg.DistributionDate = ordreStatusService.DistributionDate;

            if (!ordreStatusService.DistributionType.Equals(string.Empty))
                utvalg.DistributionType = (DistributionType)Convert.ToInt32(ordreStatusService.DistributionType);
        }

        private void SetNewValuesOnList(UtvalgList utvalgList, OrdreStatusService ordreStatusService)
        {
            utvalgList.OrdreStatus = Utils.EntityMapping(ordreStatusService.Status);

            if (ordreStatusService.Innleveringdato != null/* TODO Change to default(_) if this is not a reference type */ )
                utvalgList.InnleveringsDato = Convert.ToDateTime(ordreStatusService.Innleveringdato);

            if (ordreStatusService.Avtalenummer != null/* TODO Change to default(_) if this is not a reference type */ )
                utvalgList.Avtalenummer = ordreStatusService.Avtalenummer;

            if (ordreStatusService.Status == OrdreTilbudStatusKode.Kansellert)
            {
                utvalgList.OrdreReferanse = null;
                utvalgList.OrdreType = OrdreType.Null;
                utvalgList.Avtalenummer = 0;
            }
            else if (ordreStatusService.OEBSRef != null)
            {
                utvalgList.OrdreReferanse = ordreStatusService.OEBSRef;
                utvalgList.OrdreType = Utils.EntityMapping(ordreStatusService.OEBSType);


                if (utvalgList.MemberLists.Count > 0)
                {
                    foreach (UtvalgList list in utvalgList.MemberLists)
                    {
                        // Integration have no utvalgs in lists.Make sure the list is loaded.
                        UtvalgList uList;
                        uList = _utvalgListRepository.GetUtvalgList(list.ListId, false, true).Result;
                        // li.CalculateAntall()

                        // The utvalgs are missing reols and can not be copied without.
                        foreach (Utvalg utvalg in uList.MemberUtvalgs)
                            _utvalgRepository.GetUtvalgReolerForIntegration(utvalg).Wait();
                    }
                }

                if (utvalgList.MemberUtvalgs.Count > 0)
                {
                    // Integration have no reols in save of copies of utvalg.Make sure the utvalg is loaded.
                    foreach (Utvalg utv in utvalgList.MemberUtvalgs)
                    {
                        _utvalgRepository.GetUtvalgReolerForIntegration(utv).Wait();
                        utv.CalculateTotalAntall();
                    }
                }

                // 20110926 LHK: Posten ønsker at lister automatisk skal frikobles fra basislister i det ordre opprettes (i det PUMA mottar ordrestatusmelding med OEBS type = "O"). 
                if (utvalgList.BasedOn > 0 && Convert.ToInt32(utvalgList.OrdreType) == Convert.ToInt32(OEBSTypeKode.Ordre))
                    //KSPU.BusinessLayer.BasisUtvalgListManager.DisconnectList(l, ordreStatus.SistEndretAv);

                // 20130422 - Supportsak #622825 PUMA, basisliste knyttet til tilbud
                utvalgList.ParentList = null;
            }

            if (!ordreStatusService.DistributionDate.Equals(DateTime.MinValue))
                utvalgList.DistributionDate = ordreStatusService.DistributionDate;

            if (!ordreStatusService.DistributionType.Equals(string.Empty))
                utvalgList.DistributionType = (DistributionType)Convert.ToInt32(ordreStatusService.DistributionType);
        }

        // Metoden bygger opp en liste med Fordelinger, som tilknyttes Utvalgsdetaljene
        // Hver Fordeling har en liste med MottakerAntall
        // Mottakerantall beskriver hvor mange mottakere innenfor hver kategori det er i Fordelingen
        // En Fordeling er en mapping mot Reol, som er en beskrivelse av en postrute
        private static Utvalgsdetaljer CreateUtvalgsdetaljerForUtvalg(Utvalg utvalg)
        {
            List<Fordeling> fordelinger = new List<Fordeling>();
            foreach (Reol reol in utvalg.Reoler)
            {
               
                    AntallInformation totalAntall = reol.Antall;
                    List<MottakerAntall> res = new List<MottakerAntall>();
                    // TODO: sjekk om receivers inneholder KUN valgte receivers. Om ikke må vite benytte selected!
                    foreach (UtvalgReceiver receiver in utvalg.Receivers)
                    {
                       
                            if ((receiver.ReceiverId == ReceiverType.Farmers | receiver.ReceiverId == ReceiverType.Houses))
                            {
                                if (!utvalg.Receivers.ContainsReceiver(ReceiverType.Households))
                                {
                                    if (utvalg.Receivers.ContainsReceiver(ReceiverType.FarmersReserved) | utvalg.Receivers.ContainsReceiver(ReceiverType.HousesReserved))
                                        res.Add(new MottakerAntall(Utils.EntityMapping(receiver.ReceiverId), totalAntall.GetAntall(receiver.ReceiverId), totalAntall.GetAntallReserved(receiver.ReceiverId)));
                                    else
                                        res.Add(new MottakerAntall(Utils.EntityMapping(receiver.ReceiverId), totalAntall.GetAntall(receiver.ReceiverId), null));
                                }
                            }
                            else if (receiver.ReceiverId == ReceiverType.Households)
                            {
                                if (utvalg.Receivers.ContainsReceiver(ReceiverType.HouseholdsReserved))
                                    res.Add(new MottakerAntall(Utils.EntityMapping(receiver.ReceiverId), totalAntall.GetAntall(receiver.ReceiverId), totalAntall.GetAntallReserved(receiver.ReceiverId)));
                                else
                                    res.Add(new MottakerAntall(Utils.EntityMapping(receiver.ReceiverId), totalAntall.GetAntall(receiver.ReceiverId), null));
                            }
                            else if (receiver.ReceiverId == ReceiverType.Businesses)
                                res.Add(new MottakerAntall(Utils.EntityMapping(receiver.ReceiverId), totalAntall.GetAntall(receiver.ReceiverId), null));
                       
                    }
                    Fordeling fordeling = new Fordeling(reol.Fylke, reol.KommuneFullDistribusjon, reol.Kommune, reol.PostalZone, reol.PostalArea, reol.TeamName, reol.TeamNumber, true, reol.Name, reol.ReolNumber, reol.PrisSone, res, reol.PrsEnhetsId);
                    fordelinger.Add(fordeling);
                
            }
            Nullable<int> listId = 0;
            //if (utvalg.List != null)
            //    listId = utvalg.List.ListId;
            //else
            //    listId = null;
            if (!string.IsNullOrWhiteSpace(utvalg.ListId))
            {
                listId = Convert.ToInt32(utvalg.ListId);
            }
            return new Utvalgsdetaljer(utvalg.UtvalgId, UtvalgsTypeKode.Utvalg, (int)listId, utvalg.Name, utvalg.Logo, GetFordelingstype(utvalg), fordelinger);
        }

        private static Utvalgsdetaljer CreateUtvalgsdetaljerForUtvalgList(UtvalgList utvalgList)
        {
            int parentListId = default(int);
            if (utvalgList.ParentList != null)
                parentListId = utvalgList.ParentList.ListId;
            return new Utvalgsdetaljer(utvalgList.ListId, UtvalgsTypeKode.Liste, parentListId, utvalgList.Name, utvalgList.Logo, GetFordelingstype(utvalgList), null);
        }

        public static FordelingsTypeKode GetFordelingstype(Utvalg utvalg)
        {
            if (utvalg.IsFullDistribution())
                return FordelingsTypeKode.KommuneBydel;

            foreach (UtvalgReceiver receiver in utvalg.Receivers)
            {
                if ((receiver.ReceiverId == ReceiverType.FarmersReserved) | (receiver.ReceiverId == ReceiverType.HouseholdsReserved) | (receiver.ReceiverId == ReceiverType.HousesReserved))
                    return FordelingsTypeKode.Informasjon;
            }
            return FordelingsTypeKode.Normal;
        }

        public static FordelingsTypeKode GetFordelingstype(UtvalgList ul)
        {
            // Loop gjennom alle utvalg og sjekk status.
            // Ett utvalg med kommune/bydel --> FordelingsTypeKode.KommuneBydel
            // Ett utvalg med resreverte --> FordelingsTypeKode.Informasjon
            // Else FordelingsTypeKode.Normal
            FordelingsTypeKode fordelingstype;
            foreach (Utvalg u in ul.MemberUtvalgs)
            {
                fordelingstype = GetFordelingstype(u);
                if (fordelingstype == FordelingsTypeKode.KommuneBydel)
                    return fordelingstype;
                if (fordelingstype == FordelingsTypeKode.Informasjon)
                    return fordelingstype;
            }
            return FordelingsTypeKode.Normal;
        }

        // 'Kommet inn pga Endringsordre 16, januar 2008
        private void CheckIfAllRuterInTeam(Utvalgsfordeling utvFordeling)
        {

            // get count of unique ruter in each team and put into a hashtable
            DataTable dataTable = _utvalgRepository.GetNumberOfBudruterInTeam().Result;
            Hashtable htDBRutecountInTeam = new Hashtable();
            foreach (DataRow dataRow in dataTable.Rows)
                htDBRutecountInTeam.Add(dataRow[0], dataRow[1]);

            // loop through all Fordelinger in each Utvalgsdetaljer and get count of ruter in each team
            foreach (Utvalgsdetaljer uDet in utvFordeling.Utvalg)
            {
                if (uDet.Fordelinger != null)
                {

                    // Finner status på hva resultatet inneholder
                    Hashtable htRutecountInTeam = new Hashtable();
                    foreach (Fordeling fordeling in uDet.Fordelinger)
                    {
                        string teamid = fordeling.TeamNr;
                        if (!htRutecountInTeam.ContainsKey(teamid))
                            htRutecountInTeam.Add(teamid, 1); // legger til teamid og ett registret antall
                        else
                        {
                            long count = Convert.ToInt64(Convert.ToString(htRutecountInTeam[teamid]));
                            htRutecountInTeam[teamid] = count + 1; // legger til en til registret antall
                        }
                    }

                    // Setter property TeamKomplett (default er true)
                    // DictionaryEntry dictionaryEntry;
                    foreach (DictionaryEntry dictionaryEntry in htRutecountInTeam)
                    {
                        string teamId = dictionaryEntry.Key.ToString();
                        long teamCount = Convert.ToInt64(Convert.ToString(dictionaryEntry.Value));
                        long totCount = 0;
                        if (htDBRutecountInTeam.ContainsKey(teamId))
                            totCount = (long)htDBRutecountInTeam[teamId];
                        // om alle ruter i et team er med, oppdater alle fordelinger med dette teamet
                        if (totCount != teamCount)
                        {
                            foreach (Fordeling fordeling in uDet.Fordelinger)
                            {
                                if (fordeling.TeamNr.Equals(dictionaryEntry.Key.ToString()))
                                    fordeling.TeamKomplett = false;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
