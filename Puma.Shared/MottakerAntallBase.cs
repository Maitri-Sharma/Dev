using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public abstract class MottakerAntallBase
    {
        private int antall;

        private PumaEnum.MottakerGruppeKode mottakerGruppe;

        public int Antall
        {
            get { return antall; }
            set { antall = value; }
        }

        public PumaEnum.MottakerGruppeKode MottakerGruppe
        {
            get { return mottakerGruppe; }
            set { mottakerGruppe = value; }
        }
    }
}
