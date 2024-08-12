using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DataAccessAPI.HandleRequest.Response.UtvalgList
{
    public class ResponseCreateCopyOfUtalgList
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public int ListId { get; set; }


        /// <summary>
        /// Gets or sets the modifications.
        /// </summary>
        /// <value>
        /// The modifications.
        /// </value>
        public List<UtvalgModification> Modifications
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public string Logo { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the kunde navn.
        /// </summary>
        /// <value>
        /// The kunde navn.
        /// </value>
        public string KundeNavn { get; set; }
        /// <summary>
        /// Gets or sets the ordre referanse.
        /// </summary>
        /// <value>
        /// The ordre referanse.
        /// </value>
        public string OrdreReferanse { get; set; }
        /// <summary>
        /// Gets or sets the type of the ordre.
        /// </summary>
        /// <value>
        /// The type of the ordre.
        /// </value>
        public PumaEnum.OrdreType OrdreType { get; set; }
        /// <summary>
        /// Gets or sets the ordre status.
        /// </summary>
        /// <value>
        /// The ordre status.
        /// </value>
        public PumaEnum.OrdreStatus OrdreStatus { get; set; }
        /// <summary>
        /// Gets or sets the kunde nummer.
        /// </summary>
        /// <value>
        /// The kunde nummer.
        /// </value>
        public string KundeNummer { get; set; }
        /// <summary>
        /// Gets or sets the innleverings dato.
        /// </summary>
        /// <value>
        /// The innleverings dato.
        /// </value>
        public DateTime InnleveringsDato { get; set; }

        /// <summary>
        /// ''' Evt avtalenummer tilknyttet utvalg. Settes og benyttes av Ordre
        /// '''
        /// </summary>
        private int _Avtalenummer;
        /// <summary>
        /// Gets or sets the avtalenummer.
        /// </summary>
        /// <value>
        /// The avtalenummer.
        /// </value>
        public Nullable<int> Avtalenummer
        {
            get
            {
                return _Avtalenummer;
            }
            set
            {
                _Avtalenummer = (int)value;
            }
        }
        /// <summary>
        /// Gets or sets the sist oppdatert.
        /// </summary>
        /// <value>
        /// The sist oppdatert.
        /// </value>
        public DateTime SistOppdatert { get; set; }
        /// <summary>
        /// Gets or sets the sist endret av.
        /// </summary>
        /// <value>
        /// The sist endret av.
        /// </value>
        public string SistEndretAv { get; set; }
        /// <summary>
        /// Gets or sets the antall when last saved.
        /// </summary>
        /// <value>
        /// The antall when last saved.
        /// </value>
        public long AntallWhenLastSaved { get; set; } = 0;
        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public int Weight { get; set; }
        /// <summary>
        /// Gets or sets the type of the distribution.
        /// </summary>
        /// <value>
        /// The type of the distribution.
        /// </value>
        public PumaEnum.DistributionType DistributionType { get; set; }
        /// <summary>
        /// Gets or sets the distribution date.
        /// </summary>
        /// <value>
        /// The distribution date.
        /// </value>
        public DateTime DistributionDate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is basis.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is basis; otherwise, <c>false</c>.
        /// </value>
        public bool IsBasis { get; set; }
        /// <summary>
        /// Gets or sets the based on.
        /// </summary>
        /// <value>
        /// The based on.
        /// </value>
        public int BasedOn { get; set; }

        /// <summary>
        /// Gets or sets the name of the based on.
        /// </summary>
        /// <value>
        /// The name of the based on.
        /// </value>
        public string BasedOnName
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the was based on.
        /// </summary>
        /// <value>
        /// The was based on.
        /// </value>
        public int WasBasedOn { get; set; }

        /// <summary>
        /// Gets or sets the name of the was based on.
        /// </summary>
        /// <value>
        /// The name of the was based on.
        /// </value>
        public string WasBasedOnName
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [allow double].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow double]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowDouble { get; set; }
        /// <summary>
        /// Gets or sets the lists based on me.
        /// </summary>
        /// <value>
        /// The lists based on me.
        /// </value>
        public List<CampaignDescription> ListsBasedOnMe { get; set; }

        /// <summary>
        /// Gets or sets the member utvalgs.
        /// </summary>
        /// <value>
        /// The member utvalgs.
        /// </value>
        public List<Puma.Shared.Utvalg> MemberUtvalgs
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the member lists.
        /// </summary>
        /// <value>
        /// The member lists.
        /// </value>
        public List<Puma.Shared.UtvalgList> MemberLists
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the antall.
        /// </summary>
        /// <value>
        /// The antall.
        /// </value>
        public long Antall
        {
            get;
            //{
            //return CalculateAntall();
            //}
            set;
            // {
            // _Antall = CalculateAntall();
            // }
        }
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double Thickness { get; set; }
    }
}
