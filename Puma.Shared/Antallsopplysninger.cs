using System;
using System.Collections.Generic;
using System.Text;
using static Puma.Shared.PumaEnum;

namespace Puma.Shared
{
    public partial class Antallsopplysninger
    {
        public Antallsopplysninger()
        { }

        public Antallsopplysninger(PumaEnum.FordelingsTypeKode? fordelingsType, string utvalgsRef, 
            DateTime? sistOppdatert, int? antallSegmenter, int? antallDemografi, List<MottakerAntallSone> antallPrSone,
            int? vekt, double? tykkelse, String distribusjonsdato, PumaEnum.Distribusjonstype distribusjonstype, List<String> involvertePRS)
        {
            if (fordelingsType != null)
            {
                this.fordelingsType = (FordelingsTypeKode)fordelingsType;
            }
            else
                this.fordelingsType = FordelingsTypeKode.Null;
            this.utvalgsRef = utvalgsRef;
            this.sistOppdatert = Convert.ToDateTime(sistOppdatert);
            this.antallPrSone = antallPrSone;
            this.antallDemografi = Convert.ToInt32(antallDemografi);
            this.antallSegmenter = Convert.ToInt32(antallSegmenter);
            this.vekt = Convert.ToInt32(vekt);
            this.tykkelse = Convert.ToDouble(tykkelse);
            this.distribusjonsdato = distribusjonsdato;
            this.distribusjonstype = distribusjonstype;
            this.involvertePRS = involvertePRS;
        }
        
        public Antallsopplysninger(PumaEnum.FeilKode feilKode)
        {
            this.feilKode = feilKode;
        }

        private PumaEnum.FordelingsTypeKode fordelingsType;
        public PumaEnum.FordelingsTypeKode FordelingsType
        {
            get { return fordelingsType; }
            set { fordelingsType = value; }
        }

        private string utvalgsRef;
        public string UtvalgsRef
        {
            get { return utvalgsRef; }
            set { utvalgsRef = value; }
        }

        private DateTime sistOppdatert;
        public DateTime SistOppdatert
        {
            get { return sistOppdatert; }
            set { sistOppdatert = value; }
        }

        private PumaEnum.FeilKode feilKode;
        public PumaEnum.FeilKode FeilKode
        {
            get { return feilKode; }
            set { feilKode = value; }
        }

        private List<MottakerAntallSone> antallPrSone;
        public List<MottakerAntallSone> AntallPrSone
        {
            get { return antallPrSone; }
            set { antallPrSone = value; }
        }

        private int antallSegmenter;
        public int AntallSegmenter
        {
            get { return antallSegmenter; }
            set { antallSegmenter = value; }
        }

        private int antallDemografi;
        public int AntallDemografi
        {
            get { return antallDemografi; }
            set { antallDemografi = value; }
        }

        private int vekt;
        public int Vekt
        {
            get { return vekt; }
            set { vekt = value; }
        }

        private double tykkelse;
        public double Tykkelse
        {
            get { return tykkelse; }
            set { tykkelse = value; }
        }

        private String distribusjonsdato; //dateformat: ddmmyyyy
        public String Distribusjonsdato
        {
            get { return distribusjonsdato; }
            set { distribusjonsdato = value; }
        }

        private PumaEnum.Distribusjonstype distribusjonstype;
        public PumaEnum.Distribusjonstype Distribusjonstype
        {
            get { return distribusjonstype; }
            set { distribusjonstype = value; }
        }

        private List<String> involvertePRS;
        public List<String> InvolvertePRS
        {
            get { return involvertePRS; }
            set { involvertePRS = value; }
        }
    }
}