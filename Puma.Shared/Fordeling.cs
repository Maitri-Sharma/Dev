using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    [Serializable()]
    public partial class Fordeling
    {
        public Fordeling()
        {
        }

        public Fordeling(string fylke, string kommuneBydel, string kommuneRute, string postNr, string postSted, string team, string teamNr, bool teamKomplett, string rute, string ruteNr, int sone, List<MottakerAntall> MottakerType, String PRS)
        {
            this.fylke = fylke;
            this.kommuneBydel = kommuneBydel;
            this.kommuneRute = kommuneRute;
            this.postNr = postNr;
            this.postSted = postSted;
            this.team = team;
            this.teamNr = teamNr;
            this.teamKomplett = teamKomplett;
            this.rute = rute;
            this.ruteNr = ruteNr;
            this.sone = sone.ToString();
            this.antall = MottakerType;
            this.PRS = PRS;
        }

        private string fylke;
        public string Fylke
        {
            get { return fylke; }
            set { fylke = value; }
        }

        private string kommuneRute;
        public string KommuneRute
        {
            get { return kommuneRute; }
            set { kommuneRute = value; }
        }

        private string kommuneBydel;
        public string KommuneBydel
        {
            get { return kommuneBydel; }
            set { kommuneBydel = value; }
        }

        private string postNr;
        public string PostNr
        {
            get { return postNr; }
            set { postNr = value; }
        }

        private string postSted;
        public string PostSted
        {
            get { return postSted; }
            set { postSted = value; }
        }

        private string teamNr;
        public string TeamNr
        {
            get { return teamNr; }
            set { teamNr = value; }
        }

        private string team;
        public string Team
        {
            get { return team; }
            set { team = value; }
        }

        private bool teamKomplett;
        public bool TeamKomplett
        {
            get { return teamKomplett; }
            set { teamKomplett = value; }
        }

        private string ruteNr;
        public string RuteNr
        {
            get { return ruteNr; }
            set { ruteNr = value; }
        }

        private string rute;
        public string Rute
        {
            get { return rute; }
            set { rute = value; }
        }

        private string sone;
        public string Sone
        {
            get { return sone; }
            set { sone = value; }
        }

        private List<MottakerAntall> antall;
        public List<MottakerAntall> Antall
        {
            get { return antall; }
            set { antall = value; }
        }

        private string prs;
        public string PRS
        {
            get { return prs; }
            set { prs = value; }
        }
    }
}
