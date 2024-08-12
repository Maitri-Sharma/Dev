using Puma.Shared;
using DataAccessAPI.Models;

namespace DataAccessAPI.ECPumaHelper
{
    /// <summary>
    /// Denne klassen håndterer oversetting fra eksterne til interne entiteter.
    /// Typer som kun oversettes eksternt til internt hører hjemme her.
    /// </summary>
    public static class TranslateExternalToInternal
    {
        /// <summary>
        /// Oversetter typen OrdreStatus
        /// </summary>
        /// <param name="from">Ekstern ordrestatus</param>
        /// <returns>Intern ordrestatus</returns>
        public static OrdreStatusService OrdreStatus(OrdreStatusData from)
        {
            OrdreStatusService to = new OrdreStatusService();
            to.Avtalenummer = from.Avtalenummer;
            to.UtvalgId = from.Id;
            to.Innleveringdato = from.Innleveringsdato; 
            to.Kommentar = from.Kommentar;
            to.OEBSRef = from.OEBSRef;
            to.OEBSType = TranslateBothWays.OEBSTypeToInternal(from.OEBSType);
            to.ReturnerFordeling = from.ReturnerFordeling;
            to.SistEndretAv = from.EndretAv;
            to.Status = TranslateOrdreTilbudStatusKode(from.Status);
            to.Type = TranslateBothWays.UtvalgTypeKodeToInternal(from.Type);

            //to.DistributionType = from.OmdelingsType;

            // TBAK 2011-06-27: Endret to/from konvertering til samme som InnleveringsDato
            to.DistributionDate = from.DistribusjonsDato;

            //DateTime temp = DateTime.MinValue;
            //DateTime.TryParse(from.DistributionDate, out temp);  
            //to.DistributionDate = temp;

            //to.DistributionType = string.Empty;
            //to.DistributionDate = DateTime.MinValue;

            return to;
        }

        /// <summary>
        /// Oversetter typen OrdreTilbudStatusKode
        /// </summary>
        /// <param name="from">Bokstavkode for status</param>
        /// <returns>Enum type</returns>
        private static PumaEnum.OrdreTilbudStatusKode TranslateOrdreTilbudStatusKode(string from)
        {
            PumaEnum.OrdreTilbudStatusKode ft = PumaEnum.OrdreTilbudStatusKode.Registrert;
            switch (from)
            {
                case "G":
                    ft = PumaEnum.OrdreTilbudStatusKode.Godkjent;
                    break;
                case "I":
                    ft = PumaEnum.OrdreTilbudStatusKode.Innlevert;
                    break;
                case "K":
                    ft = PumaEnum.OrdreTilbudStatusKode.Kansellert;
                    break;
            }
            return ft;
        }
    }
}

