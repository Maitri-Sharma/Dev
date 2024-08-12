using Puma.Shared;
using System;
using System.Collections.Generic;
using static Puma.Shared.PumaEnum;
using Ext = DataAccessAPI.Models;
using Int = Puma.Shared;

namespace DataAccessAPI.ECPumaHelper
{
    /// <summary>
    /// Denne klassen håndterer oversetting fra interne til eksterne entiteter
    /// Typer som kun oversettes internt til eksternt hører hjemme her.
    /// </summary>
    public static class TranslateInternalToExternal
    {
        #region Utvalg
        /// <summary>
        /// Oversetter typen Utvalg.
        /// </summary>
        /// <param name="from">Internt utvalg</param>
        /// <returns>Ekstern type</returns>
        public static Ext.UtvalgSvc Utvalg(Int.ECPumaData from)
        {
            Ext.UtvalgSvc to = new Ext.UtvalgSvc();
            to.Antall = from.Antall;
            to.PreviewURL = from.PreviewURL;
            to.DatoOppdatert = from.SistOppdatert;
            to.Id = Convert.ToInt32(from.UtvalgId);
            to.ParentId = Convert.ToInt32(from.ParentId);
            to.Navn = from.Navn;
            to.Type = TranslateBothWays.UtvalgTypeKodeToExternal(from.Type);
            return to;
        }
        #endregion

        #region Utvalgsfordeling
        /// <summary>
        /// Oversetter typen Utvalgsfordeling.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Ext.Utvalgsfordeling Utvalgsfordeling(Int.Utvalgsfordeling from)
        {
            try
            {
                Ext.Utvalgsfordeling to = new Ext.Utvalgsfordeling();
                to.DatoOppdatert = from.SistOppdatert;
                to.Id = Convert.ToInt32(from.UtvalgId);
                to.TopplisteLogo = !string.IsNullOrWhiteSpace(from.Logo) ? from.Logo : "";
                to.OEBSRef = from.OEBSRef;
                to.OEBSType = TranslateBothWays.OEBSTypeKodeToExternal(from.OEBSType);
                to.UtvalgsRef = from.UtvalgsRef;
                to.Type = TranslateBothWays.UtvalgTypeKodeToExternal(from.Type);

                if (from.Utvalg != null && from.Utvalg.Count != 0)
                {
                    to.Utvalgsdetaljer = new Ext.Helper.Utvalgsdetaljer();

                    foreach (Int.Utvalgsdetaljer detaljer in from.Utvalg)
                    {
                        to.Utvalgsdetaljer.Add(TranslateUtvalgDetaljer(detaljer));
                    }
                }
                return to;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Oversetter typen Utvalgsdetaljer.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private static Ext.UtvalgDetaljer TranslateUtvalgDetaljer(Int.Utvalgsdetaljer from)
        {
            //Tracer.Instance.TraceVerbose("FordelingsType == {0}", from.FordelingsType);
            Ext.UtvalgDetaljer to = new Ext.UtvalgDetaljer();
            to.Fordelingstype = TranslateFordelingsType(from.FordelingsType);
            to.Id = Convert.ToInt32(from.UtvalgId);
            to.Logo = from.Logo;
            to.Navn = from.Navn;
            to.ParentId = Convert.ToInt32(from.ParentId);
            to.Type = TranslateBothWays.UtvalgTypeKodeToExternal(from.Type);

            //if (from.Fordelinger == null)
            //    Tracer.Instance.TraceVerbose("Id={0}, Type={1}, Parent={2}, Navn={3} har ingen fordelinger",
            //        from.Id.ToString(), from.Type.ToString(), from.ParentId.ToString(), from.Navn);

            if (from.Fordelinger != null && from.Fordelinger.Count != 0)
            {
                //Tracer.Instance.TraceVerbose("Id={0}, Type={1}, Parent={2}, Navn={3} har {4} fordelinger",
                //    from.Id.ToString(), from.Type.ToString(), from.ParentId.ToString(), from.Navn, from.Fordelinger.Count.ToString());

                to.Fordelinger = new Ext.Helper.Fordelinger();

                foreach (Int.Fordeling fordeling in from.Fordelinger)
                {
                    to.Fordelinger.Add(TranslateFordeling(fordeling));
                }
            }
            return to;
        }

        /// <summary>
        /// Oversetter typen Fordeling.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private static Ext.Fordeling TranslateFordeling(Int.Fordeling from)
        {
            Ext.Fordeling to = new Ext.Fordeling();
            to.Fylke = from.Fylke;
            to.KommuneBydel = from.KommuneBydel;
            to.KommuneRute = from.KommuneRute;
            to.Postnummer = from.PostNr;
            to.Poststed = from.PostSted;
            to.Rute = from.Rute;
            to.RuteNr = from.RuteNr;
            to.Sone = from.Sone;
            to.Team = from.Team;
            to.TeamNr = from.TeamNr;
            to.TeamKomplett = from.TeamKomplett ? "Y" : "N";
            to.PRS = from.PRS;

            if (from.Antall != null && from.Antall.Count != 0)
            {
                to.Antall = new Ext.Helper.Antall();

                foreach (Int.MottakerAntall antall in from.Antall)
                {
                    to.Antall.Add(TranslateInternalToExternal.TranslateMottakerAntall(antall));
                }
            }
            return to;
        }

        /// <summary>
        /// Oversetter typen MottakerAntall.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private static Ext.MottakerAntall TranslateMottakerAntall(Int.MottakerAntall from)
        {
            Ext.MottakerAntall to = new Ext.MottakerAntall();
            to.MottakerType = TranslateMottakerGruppe(from.MottakerGruppe);
            to.Antall = from.Antall;
            to.Reserverte = from.Reserverte;
            return to;
        }
        #endregion

        #region Utvalgantall
        /// <summary>
        /// Oversetter typen Antallsopplysninger.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Ext.Utvalgantall AntallsopplysningerToUtvalgAntall(Int.Antallsopplysninger from)
        {
            Ext.Utvalgantall to = new Ext.Utvalgantall();

            to.AntallDemografi = from.AntallDemografi;
            to.AntallSegmenter = from.AntallSegmenter;
            to.DatoOppdatert = from.SistOppdatert;
            to.FordelingsType = TranslateFordelingsType(from.FordelingsType);
            to.Utvalgsref = from.UtvalgsRef;

            to.Vekt = from.Vekt;
            to.DistribusjonssDato =TranslateDateDistribusjonssDato(from.Distribusjonsdato);
            to.Tykkelse = from.Tykkelse;
            to.DistribusjonsType = from.Distribusjonstype.ToString();
            //to.DistribusjonsType = TranslateDistribusjonsType(from.Distribusjonstype);


            to.UtvalgPRSListe = TranslateInvolvertePRS(from.InvolvertePRS);

            if (from.AntallPrSone != null && from.AntallPrSone.Count != 0)
            {
                to.AntallSone = new Ext.Helper.SoneAntall();
                foreach (Int.MottakerAntallSone antallSone in from.AntallPrSone)
                {
                    to.AntallSone.Add(TranslateInternalToExternal.TranslateMottakerAntallSone(antallSone));
                }
            }
            return to;
        }

        //private static string TranslateDistribusjonsType(Distribusjonstype distribusjonstype)
        //{
        //    string returnValue = null;

        //    switch (distribusjonstype)
        //    {
        //        case Distribusjonstype.Null:
        //            returnValue = string.Empty;
        //            break;
        //        case Distribusjonstype.B:
        //            returnValue = "B";
        //            break;
        //        case Distribusjonstype.S:
        //            returnValue = "S";
        //            break;
        //    }

        //    return returnValue;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="involvertePRSer"></param>
        /// <returns></returns>
        private static Ext.Helper.UtvalgPRSListe TranslateInvolvertePRS(List<string> involvertePRSer)
        {
            Ext.Helper.UtvalgPRSListe prsListe = new Ext.Helper.UtvalgPRSListe();

            foreach (string involvertePRS in involvertePRSer)
            {
                Ext.PRS prsNummer = new Ext.PRS();
                prsNummer.PRSNr = involvertePRS;
                prsListe.Add(prsNummer);
            }

            return prsListe;
        }

        /// <summary>
        /// Oversetter typen MottakerAntallSone.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private static Ext.MottakerAntallSone TranslateMottakerAntallSone(Int.MottakerAntallSone from)
        {
            Ext.MottakerAntallSone to = new Ext.MottakerAntallSone();
            to.Antall = from.Antall;
            to.Sone = from.Sone;
            to.MottakerType = TranslateMottakerGruppe(from.MottakerGruppe);
            return to;
        }
        #endregion

        #region Shared
        /// <summary>
        /// Oversetter typen MottakerGruppeKode.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private static string TranslateMottakerGruppe(PumaEnum.MottakerGruppeKode from)
        {
            string mt = null;
            switch (from)
            {
                case PumaEnum.MottakerGruppeKode.Boligblokker:
                    mt = "BLOKK";
                    break;
                case PumaEnum.MottakerGruppeKode.EneboligerOgRekkehus:
                    mt = "ENEBOLIG_REKKEHUS";
                    break;
                case PumaEnum.MottakerGruppeKode.Gardbrukere:
                    mt = "GÅRDSBRUK";
                    break;
                case PumaEnum.MottakerGruppeKode.Husholdninger:
                    mt = "HUSHOLDN";
                    break;
                case PumaEnum.MottakerGruppeKode.Virksomheter:
                    mt = "VIRKSOMHETER";
                    break;
            }
            return mt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime TranslateDateDistribusjonssDato(string date)
        {
            DateTime returnValue = DateTime.MinValue;

            // formatet "ddMMyyyy".

            int day = 0;
            int month = 0;
            int year = 0;

            int.TryParse(date.Substring(0, 2), out day);
            int.TryParse(date.Substring(2, 2), out month);
            int.TryParse(date.Substring(4, 4), out year);

            returnValue = new DateTime(year, month, day);

            return returnValue;
        }

        /// <summary>
        /// Oversetter typen FordelingsTypeKode.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private static string TranslateFordelingsType(PumaEnum.FordelingsTypeKode from)
        {
            string ft = null;
            switch (from)
            {
                case PumaEnum.FordelingsTypeKode.Informasjon:
                    ft = "I";
                    break;
                case PumaEnum.FordelingsTypeKode.KommuneBydel:
                    ft = "K";
                    break;
                default:           // FordelingsTypeKode.Normal
                    ft = "N";
                    break;
            }
            return ft;
        }
        #endregion
    }
}

