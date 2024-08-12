using Ext = DataAccessAPI.Models;
using Int = Puma.Shared;

namespace DataAccessAPI.ECPumaHelper
{
    /// <summary>
    /// Denne klassen håndterer oversetting mellom eksterne og interne entiteter. Typer som skal oversettes begge veier hører hjemme her.
    /// </summary>
    public static class TranslateBothWays
    {
        /// <summary>
        /// Oversetter typen UtvalgsId til ekstern type.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Ext.UtvalgsId UtvalgsIdToExternal(Int.UtvalgsId from)
        {
            Ext.UtvalgsId to = new Ext.UtvalgsId();
            to.Id = (int)from.UtvalgId; 
            to.Type = TranslateBothWays.UtvalgTypeKodeToExternal(from.Type);
            return to;
        }

        /// <summary>
        /// Oversetter typen UtvalgsId til intern type.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Int.UtvalgsId UtvalgsIdToInternal(Ext.UtvalgsId from)
        {
            Int.UtvalgsId to = new Int.UtvalgsId();
            to.UtvalgId = from.Id;
            to.Type = TranslateBothWays.UtvalgTypeKodeToInternal(from.Type);
            return to;
        }

        /// <summary>
        /// Oversetter typen OEBSTypeKode til ekstern type.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Ext.Helper.UtvalgsTypeKode UtvalgTypeKodeToExternal(Int.PumaEnum.UtvalgsTypeKode from)
        {
            Ext.Helper.UtvalgsTypeKode retVal = Ext.Helper.UtvalgsTypeKode.Utvalg;

            if (from == Int.PumaEnum.UtvalgsTypeKode.Liste)
                retVal = Ext.Helper.UtvalgsTypeKode.Liste;

            return retVal;
        }

        /// <summary>
        /// Oversetter typen OEBSTypeKode til intern type.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Int.PumaEnum.UtvalgsTypeKode UtvalgTypeKodeToInternal(Ext.Helper.UtvalgsTypeKode from)
        {
            Int.PumaEnum.UtvalgsTypeKode retVal = Int.PumaEnum.UtvalgsTypeKode.Utvalg;

            if (from == Ext.Helper.UtvalgsTypeKode.Liste)
                retVal = Int.PumaEnum.UtvalgsTypeKode.Liste;

            return retVal;
        }

        /// <summary>
        /// Oversetter typen OEBSTypeKode til intern type.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Int.PumaEnum.OEBSTypeKode OEBSTypeToInternal(string from)
        {
            Int.PumaEnum.OEBSTypeKode ft = Int.PumaEnum.OEBSTypeKode.Tilbud;
            if (from == "O")
                ft = Int.PumaEnum.OEBSTypeKode.Ordre;
            return ft;
        }

        /// <summary>
        /// Oversetter typen OEBSTypeKode til ekstern type.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static string OEBSTypeKodeToExternal(Int.PumaEnum.OEBSTypeKode from)
        {
            string retVal = "T";    //Default til tilbud
            if (from == Int.PumaEnum.OEBSTypeKode.Ordre)
                retVal = "O";
            return retVal;
        }
    }
}

