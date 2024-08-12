using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public partial class MottakerAntallSone : MottakerAntallBase
    {
        public MottakerAntallSone()
        {}

        public MottakerAntallSone(PumaEnum.MottakerGruppeKode mottakerGruppe, string sone, int antall)
        {
            base.MottakerGruppe = mottakerGruppe;
            this.sone = sone;
            base.Antall = antall;
        }

        private string sone;
        public string Sone
        {
            get { return sone; }
            set { sone = value; }
        }




    }
}
