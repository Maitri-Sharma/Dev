using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.EC_Data
{
    public partial class OrdreStatusDataEntity : UtvalgsIdEntity
    {
        private System.String kildeSystem;

        public System.String Kildesystem
        {
            get { return kildeSystem; }
            set { kildeSystem = value; }
        }

        private System.String OEBSTypeField;

        public System.String OEBSType
        {
            get { return OEBSTypeField; }
            set { OEBSTypeField = value; }
        }

        private System.String OEBSRefField;

        public System.String OEBSRef
        {
            get { return OEBSRefField; }
            set { OEBSRefField = value; }
        }

        private System.String StatusField;

        public System.String Status
        {
            get { return StatusField; }
            set { StatusField = value; }
        }

        private System.String KommentarField;

        public System.String Kommentar
        {
            get { return KommentarField; }
            set { KommentarField = value; }
        }

        private System.DateTime InnleveringsdatoField;

        public System.DateTime Innleveringsdato
        {
            get { return InnleveringsdatoField; }
            set { InnleveringsdatoField = value; }
        }

        private System.String EndretAvField;

        public System.String EndretAv
        {
            get { return EndretAvField; }
            set { EndretAvField = value; }
        }

        private System.Boolean ReturnerFordelingField;

        public System.Boolean ReturnerFordeling
        {
            get { return ReturnerFordelingField; }
            set { ReturnerFordelingField = value; }
        }

        private System.Int32 AvtalenummerField;

        public System.Int32 Avtalenummer
        {
            get { return AvtalenummerField; }
            set { AvtalenummerField = value; }
        }

        // TBAK 2011-06-27: Lagt tilbake DateTime type, endret til norsk navn samsvarende med eConnect
        private System.DateTime DistribusjonsDatoField; // = DateTime.MinValue;

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

        public string OmdelingsType
        {
            get { return OmdelingsTypeField; }
            set { OmdelingsTypeField = value; }
        }

    }
}
