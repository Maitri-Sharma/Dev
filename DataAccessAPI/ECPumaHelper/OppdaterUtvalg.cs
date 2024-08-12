using DataAccessAPI;
using DataAccessAPI.Controllers;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.ECPumaHelper
{
    public class OppdaterUtvalg
    {
        #region Variables
        private readonly ILogger<OppdaterUtvalg> _logger;
        private readonly UtvalgController utvalgController;
        private readonly UtvalgListController utvalgListController;
        private readonly MappingUtvalgsdetaljer mappingUtvalgsdetaljer;
        private readonly IUtvalgRepository _utvalgRepository;
        private readonly IUtvalgListRepository _utvalgListRepository;

        private readonly IKapasitetRepository _kapasitetRepository;



        #endregion

        #region Constructors        
       
       public OppdaterUtvalg(ILogger<OppdaterUtvalg> logger, ILogger<MappingUtvalgsdetaljer> loggerMappingUtvalgDetaljer, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList, ILogger<KapasitetController> loggerkapasitet,
            IUtvalgRepository utvalgRepository, IKapasitetRepository kapasitetRepository, IUtvalgListRepository utvalgListRepository)
        {
            _logger = logger;
            _utvalgRepository = utvalgRepository;
            _kapasitetRepository = kapasitetRepository;
            _utvalgListRepository = utvalgListRepository;

            utvalgController = new UtvalgController(loggerUtvalg, loggerConfig, loggerreol);
            utvalgListController = new UtvalgListController(loggerUtvalgList, loggerConfig, loggerUtvalg,  loggerreol,_utvalgRepository);
            mappingUtvalgsdetaljer = new MappingUtvalgsdetaljer(loggerMappingUtvalgDetaljer, loggerConfig, loggerUtvalg, loggerreol, loggerUtvalgList, loggerkapasitet, _utvalgRepository,_kapasitetRepository,_utvalgListRepository);
        }
        #endregion


        #region Public Methods

        /// <summary>
        ///     ''' OppdaterUtvalg kalles av Ordre2 ved 3 alternative forløp:
        ///     ''' A)Ordre/Tilbud registrert. Registrer Ordre/Tilbud og returner Utvalgsfordeling.
        ///     ''' B)Endring av Ordre/Tilbud. Registrer endring og returner Utvalgsfordeling.
        ///     ''' C)Endring av status på en eksisterende ordre i KSPU. Ingen retur.
        ///     ''' </summary>
        ///     ''' <param name="ordreStatusService"></param>
        ///     ''' <returns>Utvalgsdetaljer</returns>
        ///     ''' <remarks></remarks>
        public Utvalgsfordeling OppdaterUtvalgIntegrasjon(OrdreStatusService ordreStatusService)
        {
            _logger.LogDebug("Inside into OppdaterUtvalgIntegrasjon");
            // sjekk innparametre
            if (ordreStatusService.UtvalgId == null/* TODO Change to default(_) if this is not a reference type */ )
                throw new ArgumentNullException("Parameter 'ID' er ikke angitt med noen verdi.");
            if (ordreStatusService.OEBSRef == null)
                throw new ArgumentNullException("Parameter 'OEBSRef' er ikke angitt med noen verdi.");
            if (ordreStatusService.Innleveringdato == null/* TODO Change to default(_) if this is not a reference type */ )
                throw new ArgumentNullException("Parameter 'Innleveringdato' er ikke angitt med noen verdi.");
            if (ordreStatusService.SistEndretAv == null)
                throw new ArgumentNullException("Parameter 'SistEndretAv' er ikke angitt med noen verdi.");

            // Get UtvalgCollection
            UtvalgAndUtvalgListCollections ulColl = mappingUtvalgsdetaljer.GetCollectionsFromOrdreStatus(ordreStatusService);

            if (ulColl.UtvalgLists.Count == 0 && ulColl.Utvalgs.Count == 0)
                throw new NotSupportedException("Ingen utvalg finnes i KSPU for ID= {ordreStatusService.Id}, og TYPE= {ordreStatusService.Type.ToString}");

            Utvalgsfordeling utvFordeling = null/* TODO Change to default(_) if this is not a reference type */;
            // tidligere med mer logikk, som er fjernet etterhvert. Se History 27.10.09.
            if (ordreStatusService.ReturnerFordeling)
            {
                // Case a) og b)
                // Opprett Utvalgsfordeling for retur
                utvFordeling = mappingUtvalgsdetaljer.MapKSPUCollectionToWSUtvalgsfordeling(ulColl, ordreStatusService);
                // Lagre ordrereferansen. Oppdaterer info etter respons er hentet ut
                mappingUtvalgsdetaljer.DBUpdateUtvalgAndListCollection(ulColl, ordreStatusService);
                _logger.LogDebug("Exiting From OppdaterUtvalgIntegrasjon");
                return utvFordeling;
            }
            else
            {
                mappingUtvalgsdetaljer.DBUpdateUtvalgAndListCollection(ulColl, ordreStatusService);
                _logger.LogDebug("Exiting From OppdaterUtvalgIntegrasjon");
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
        }

        /// <summary>
        ///     ''' GetUtvalgsfordeling kalles av OEBS etter en Ruteoppdatering er skejdd. 
        ///     ''' Denne metoden kalles av Ergo service som henter ut data(via denne metoden) og 
        ///     ''' sender data til OEBS for alle kandidater sendt av egen exe-fil i Rutegenerator. Ergo service kjører altså i en løkke.
        ///     ''' </summary>
        ///     ''' <param name="id"></param>
        ///     ''' <param name="type"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public Utvalgsfordeling HentUtvalgsfordeling(int id, UtvalgsTypeKode type)
        {
            try
            {
                _logger.LogDebug("Inside into HentUtvalgsfordeling");
                // sjekk innparametre
                if (id == default(int))
                    throw new ArgumentNullException("Parameter 'ID' er ikke angitt med noen verdi.");

                // Get UtvalgCollection
                UtvalgAndUtvalgListCollections ulColl = mappingUtvalgsdetaljer.GetCollectionsFromIdType(id, type, true);

                if (ulColl.UtvalgLists.Count == 0 && ulColl.Utvalgs.Count == 0)
                    throw new NotSupportedException($"Ingen utvalg finnes i KSPU for ID={id} og TYPE= {type.ToString()}");

                _logger.LogInformation("Data Returned: " + ulColl);
                _logger.LogDebug("Exiting From HentUtvalgsfordeling");
                return mappingUtvalgsdetaljer.MapKSPUCollectionToWSUtvalgsfordelingByIdType(ulColl, id, type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion
    }

    public class UtvalgAndUtvalgListCollections
    {
        public UtvalgCollection Utvalgs = new UtvalgCollection();
        public UtvalgsListCollection UtvalgLists = new UtvalgsListCollection();
    }
}
