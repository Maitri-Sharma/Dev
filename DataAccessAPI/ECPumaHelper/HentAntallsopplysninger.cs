using DataAccessAPI;
using DataAccessAPI.Controllers;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.ECPumaHelper
{
    public class HentAntallsopplysninger
    {
        #region Variables
        private readonly ILogger<HentAntallsopplysninger> _logger;
        private readonly UtvalgController utvalgController;
        private readonly UtvalgListController utvalgListController;
        private readonly MappingAntallsopplysninger mappingAntall;
        private readonly IUtvalgRepository _utvalgRepository;
        private readonly IUtvalgListRepository _utvalgListRepository;

        private readonly ConfigController configController;
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="HentAntallsopplysninger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerMapping">The logger mapping.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="loggerUtvalg">The logger utvalg.</param>
        /// <param name="loggerreol">The loggerreol.</param>
        /// <param name="loggerUtvalgList">The logger utvalg list.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        public HentAntallsopplysninger(ILogger<HentAntallsopplysninger> logger, ILogger<MappingAntallsopplysninger> loggerMapping, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList,
            IUtvalgRepository utvalgRepository, IUtvalgListRepository utvalgListRepository)
        {
            _logger = logger;
            _utvalgRepository = utvalgRepository;
            _utvalgListRepository = utvalgListRepository;

            utvalgController = new UtvalgController(loggerUtvalg, loggerConfig, loggerreol);
            utvalgListController = new UtvalgListController(loggerUtvalgList, loggerConfig, loggerUtvalg,  loggerreol,_utvalgRepository);
            mappingAntall = new MappingAntallsopplysninger(loggerMapping, loggerConfig, loggerUtvalg, loggerreol, loggerUtvalgList, _utvalgRepository,_utvalgListRepository);
            configController = new ConfigController(loggerConfig);

        }
        #endregion

        #region Public Methods

        public List<Antallsopplysninger> HentAntallsopplysningerIntegrasjon(UtvalgsId utvalgsid, ref System.Nullable<FeilKode> feilKode)
        {
            _logger.LogDebug("Inside into HentAntallsopplysningerIntegrasjon");
            List<Antallsopplysninger> antallsopplysningerliste = new List<Antallsopplysninger>();
            if (utvalgsid.UtvalgId != null/* TODO Change to default(_) if this is not a reference type */ )
            {
                if (utvalgsid.Type == UtvalgsTypeKode.Utvalg)
                    antallsopplysningerliste = mappingAntall.ConvertUtvalgToWSAntallsopplysninger(_utvalgRepository.SearchUtvalgByUtvalgId(Convert.ToInt32(utvalgsid.UtvalgId)).Result);
                else if (utvalgsid.Type == UtvalgsTypeKode.Liste)
                    antallsopplysningerliste = mappingAntall.ConvertUtvalgToWSAntallsopplysninger(_utvalgListRepository.SearchUtvalgListWithChildrenById(Convert.ToInt32(utvalgsid.UtvalgId)).Result);
            }
            if (antallsopplysningerliste.Count == 0)
                feilKode = new Nullable<FeilKode>(FeilKode.UtvalgEksistererIkke);

            _logger.LogDebug("Exiting From HentAntallsopplysningerIntegrasjon");
            return antallsopplysningerliste;
        }

        #endregion

    }
}
