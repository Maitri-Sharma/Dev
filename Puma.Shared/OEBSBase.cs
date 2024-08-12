using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public abstract class OEBSBase : UtvalgsId
    {
        private string oebsRef;

        private PumaEnum.OEBSTypeKode oebsType;

        /// <summary>
        /// Referanse til ordre eller tilbud i OEBS
        /// </summary>
        public string OEBSRef
        {
            get { return this.oebsRef; }
            set { this.oebsRef = value; }
        }

        /// <summary>
        /// Angir om utvalg er knyttet til tilbud eller ordre.
        /// </summary>
        public PumaEnum.OEBSTypeKode OEBSType
        {
            get { return this.oebsType; }
            set { this.oebsType = value; }
        }
    }
}
