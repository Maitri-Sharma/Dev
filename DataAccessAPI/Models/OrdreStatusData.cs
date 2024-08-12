using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - OrdreStatus
    /// </summary>
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "OrdreStatus")]
    public partial class OrdreStatusData : UtvalgsId
    {
        private System.String kildeSystem;

        [DataMember(IsRequired = true, Name = "Kildesystem", Order = 0)]
        public System.String Kildesystem
        {
            get { return kildeSystem; }
            set { kildeSystem = value; }
        }

        private System.String OEBSTypeField;

        [DataMember(IsRequired = true, Name = "OEBSType", Order = 1)]
        public System.String OEBSType
        {
            get { return OEBSTypeField; }
            set { OEBSTypeField = value; }
        }

        private System.String OEBSRefField;

        [DataMember(IsRequired = true, Name = "OEBSRef", Order = 2)]
        public System.String OEBSRef
        {
            get { return OEBSRefField; }
            set { OEBSRefField = value; }
        }

        private System.String StatusField;

        [DataMember(IsRequired = true, Name = "Status", Order = 3)]
        public System.String Status
        {
            get { return StatusField; }
            set { StatusField = value; }
        }

        private System.String KommentarField;

        [DataMember(IsRequired = false, Name = "Kommentar", Order = 4)]
        public System.String Kommentar
        {
            get { return KommentarField; }
            set { KommentarField = value; }
        }

        private System.DateTime InnleveringsdatoField;

        [DataMember(IsRequired = false, Name = "Innleveringsdato", Order = 5)]
        public System.DateTime Innleveringsdato
        {
            get { return InnleveringsdatoField; }
            set { InnleveringsdatoField = value; }
        }

        private System.String EndretAvField;

        [DataMember(IsRequired = true, Name = "EndretAv", Order = 6)]
        public System.String EndretAv
        {
            get { return EndretAvField; }
            set { EndretAvField = value; }
        }

        private System.Boolean ReturnerFordelingField;

        [DataMember(IsRequired = true, Name = "ReturnerFordeling", Order = 7)]
        public System.Boolean ReturnerFordeling
        {
            get { return ReturnerFordelingField; }
            set { ReturnerFordelingField = value; }
        }

        private System.Int32 AvtalenummerField;

        [DataMember(IsRequired = false, Name = "Avtalenummer", Order = 8)]
        public System.Int32 Avtalenummer
        {
            get { return AvtalenummerField; }
            set { AvtalenummerField = value; }
        }

        // TBAK 2011-06-27: Lagt tilbake DateTime type, endret til norsk navn samsvarende med eConnect
        private System.DateTime DistribusjonsDatoField; // = DateTime.MinValue;

        [DataMember(IsRequired = false, Name = "DistribusjonsDato", Order = 9)]
        public DateTime DistribusjonsDato
        {
            get { return DistribusjonsDatoField; }
            set { DistribusjonsDatoField = value; }
        }


        //private string DistributionDateField = string.Empty;

        //[DataMember(IsRequired = false, Name = "DistributionDate", Order = 9)]
        //public string DistributionDate
        //{
        //    get { return DistributionDateField; }
        //    set { DistributionDateField = value; }
        //}

        // TBAK 2011-06-27: Endret til norsk navn samsvarende med eConnect
        private string OmdelingsTypeField = string.Empty;

        [DataMember(IsRequired = false, Name = "OmdelingsType", Order = 10)]
        public string OmdelingsType
        {
            get { return OmdelingsTypeField; }
            set { OmdelingsTypeField = value; }
        }

    }
}
