using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    [Serializable]
    public class Utvalgsfordeling : OEBSBase
    {
        private string logo;
        private string utvalgsRef;
        private DateTime sistOppdatert;
        private List<Utvalgsdetaljer> utvalgsdetaljer;
      
        public Utvalgsfordeling()
        {
        }

        public Utvalgsfordeling(int id, PumaEnum.UtvalgsTypeKode type, string oebsRef, PumaEnum.OEBSTypeKode oebsType, string utvalgsRef,
            DateTime sistOppdatert, List<Utvalgsdetaljer> utvalgsdetaljer) :
            this(id, type, oebsRef, oebsType, utvalgsRef, sistOppdatert,utvalgsdetaljer,null) {}

        public Utvalgsfordeling(int id, PumaEnum.UtvalgsTypeKode type, string oebsRef, PumaEnum.OEBSTypeKode oebsType, string utvalgsRef,
            DateTime sistOppdatert, List<Utvalgsdetaljer> utvalgsdetaljer, string logo)
        {
            this.logo = logo;
            base.UtvalgId = id;
            base.Type = type;
            base.OEBSRef = oebsRef;
            base.OEBSType = oebsType;
            this.utvalgsRef = utvalgsRef;
            this.sistOppdatert = sistOppdatert;
            this.utvalgsdetaljer = utvalgsdetaljer;
        }

        /// <summary>
        /// Navn på liste / utvalg (toppnivå).
        /// </summary>
        public string UtvalgsRef
        {
            get { return utvalgsRef; }
            set { utvalgsRef = value; }
        }

        public DateTime SistOppdatert
        {
            get { return sistOppdatert; }
            set { sistOppdatert = value; }
        }

        public List<Utvalgsdetaljer> Utvalg
        {
            get { return utvalgsdetaljer; }
            set { utvalgsdetaljer = value; }
        }

        public string Logo
        {
            get { return logo; }
            set { logo = value; }
        }
    }
}
