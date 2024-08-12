using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public class OrdreStatusService : OEBSBase
    {
        private PumaEnum.OrdreTilbudStatusKode status;
        private DateTime? innleveringsDato;
        private string sistEndretAv;
        private bool returnerFordeling;
        private string kommentar;
        private int? avtalenummer;
        private DateTime distributionDate = DateTime.MinValue;
        private string distributionType = string.Empty;

        public OrdreStatusService()
        {
        }

        public OrdreStatusService(int id, PumaEnum.UtvalgsTypeKode type, PumaEnum.OEBSTypeKode oebsType, string oebsRef, PumaEnum.OrdreTilbudStatusKode status,
            string kommentar, DateTime? innleveringsDato, string sistEndretAv, int avtalenummer, bool returnerFordeling)
        {
            base.UtvalgId = id;
            base.OEBSRef = oebsRef;
            base.OEBSType = oebsType;
            this.status = status;
            base.Type = type;
            this.innleveringsDato = innleveringsDato;
            this.sistEndretAv = sistEndretAv;
            this.returnerFordeling = returnerFordeling;
            this.kommentar = kommentar;
            this.avtalenummer = avtalenummer;
        }

        /// <summary>
        /// I denne meldingen vil de komme med status registrert eller Godkjent.
        /// </summary>
        public PumaEnum.OrdreTilbudStatusKode Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        /// <summary>
        /// Avtalt innleveringsdato.
        /// </summary>
        public DateTime? Innleveringdato
        {
            get { return this.innleveringsDato; }
            set { this.innleveringsDato = value; }
        }

        /// <summary>
        /// Brukerid for sist endret.
        /// </summary>
        public string SistEndretAv
        {
            get { return this.sistEndretAv; }
            set { this.sistEndretAv = value; }
        }

        /// <summary>
        /// Angir om fordelinger for utvalget skal returneres.
        /// </summary>
        public bool ReturnerFordeling
        {
            get { return this.returnerFordeling; }
            set { this.returnerFordeling = value; }
        }

        /// <summary>
        /// Eventuell kommentar.
        /// </summary>
        public string Kommentar
        {
            get { return this.kommentar; }
            set { this.kommentar = value; }
        }

        /// <summary>
        /// Eventuelt Avtalenummer.
        /// </summary>
        public int? Avtalenummer
        {
            get { return this.avtalenummer; }
            set { this.avtalenummer = value; }
        }

        public string DistributionType
        {
            get { return distributionType; }
            set { distributionType = value; }
        }

        public DateTime DistributionDate
        {
            get { return distributionDate; }
            set { distributionDate = value; }
        }
    }
}
