using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.ECPumaHelper
{
    public class Utils
    {
        //#region Variables
        //private readonly ILogger<Utils> _logger;
        //#endregion

        //#region Constructors
        ///// <summary>
        ///// Paramaterized Constructor
        ///// </summary>
        ///// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        ///// <param name="loggerConfig">Instance of Microsoft.Extensions.Logging</param>
        ///// <param name="loggerreoltable">Instance of Microsoft.Extensions.Logging</param>
        ///// <param name="loggerreol">Instance of Microsoft.Extensions.Logging</param>
        //public Utils(ILogger<Utils> logger)
        //{
        //    _logger = logger;
        //}
        //#endregion

        #region Public Methods

        public static OrdreTilbudStatusKode EntityMapping(OrdreStatus ordreStatus)
        {
            string strType = ordreStatus.ToString().ToUpper();
            foreach (OrdreTilbudStatusKode type in OrdreTilbudStatusKode.GetValues(typeof(OrdreTilbudStatusKode)))
            {
                if (strType.Equals(type.ToString().Substring(0, 1).ToUpper()))
                    return type;
            }
            // om ingen treff, sett default
            return OrdreTilbudStatusKode.Registrert;
        }

        public static MottakerGruppeKode EntityMapping(ReceiverType receiverType)
        {
            switch (receiverType)
            {
                case ReceiverType.Businesses:
                    {
                        return MottakerGruppeKode.Virksomheter;
                    }

                case ReceiverType.Farmers:
                    {
                        return MottakerGruppeKode.Gardbrukere;
                    }

                case ReceiverType.Households:
                    {
                        return MottakerGruppeKode.Husholdninger;
                    }

                case ReceiverType.Houses:
                    {
                        return MottakerGruppeKode.EneboligerOgRekkehus;
                    }
            }
            return 0;
        }

        public static OEBSTypeKode EntityMapping(OrdreType ordreType)
        {
            string strType = ordreType.ToString().ToUpper();
            foreach (OEBSTypeKode type in OEBSTypeKode.GetValues(typeof(OEBSTypeKode)))
            {
                if (strType.Equals(type.ToString().Substring(0, 1).ToUpper()))
                    return type;
            }
            // om ingen treff, sett default
            return OEBSTypeKode.Tilbud;
        }

        public static Distribusjonstype EntityMapping(DistributionType distributionType)
        {
            string strType = distributionType.ToString().ToUpper();
            foreach (Distribusjonstype type in Distribusjonstype.GetValues(typeof(Distribusjonstype)))
            {
                if (strType.Equals(type.ToString().Substring(0, 1).ToUpper()))
                    return type;
            }
            // om ingen treff, sett default
            return Distribusjonstype.Null;
        }

        public static OrdreStatus EntityMapping(OrdreTilbudStatusKode WSOrdreStatus)
        {
            string strType = WSOrdreStatus.ToString().Substring(0, 1).ToUpper();
            foreach (OrdreStatus type in OrdreStatus.GetValues(typeof(OrdreStatus)))
            {
                if (strType.Equals(type.ToString().ToUpper()))
                    return type;
            }
            // om ingen treff, sett default
            return OrdreStatus.Null;
        }

        public static OrdreType EntityMapping(OEBSTypeKode WSOrdreType)
        {
            string strType = WSOrdreType.ToString().Substring(0, 1).ToUpper();
            foreach (OrdreType type in OrdreType.GetValues(typeof(OrdreType)))
            {
                if (strType.Equals(type.ToString().ToUpper()))
                    return type;
            }
            // om ingen treff, sett default
            return OrdreType.Null;
        }


        #endregion
    }
}
