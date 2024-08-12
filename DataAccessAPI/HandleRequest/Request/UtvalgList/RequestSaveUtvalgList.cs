using MediatR;
using System;
using Puma.Shared;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSaveUtvalgList : IRequest<ResponseSearchUtvalgListSimpleById>
    {
        private int _ListId;

        public int ListId
        {
            get
            {
                return _ListId;
            }
            set
            {
                _ListId = value;
            }
        }


        public int ParentListId { get; set; }




        public string Logo { get; set; }




        public string Name { get; set; }



        // public string KundeNavn { get; set; }


        public string OrdreReferanse { get; set; }


        public PumaEnum.OrdreType OrdreType { get; set; }


        public PumaEnum.OrdreStatus OrdreStatus { get; set; }


        public string KundeNummer { get; set; }


        public DateTime InnleveringsDato { get; set; }

        /// <summary>
        ///     ''' Evt avtalenummer tilknyttet utvalg. Settes og benyttes av Ordre
        ///     ''' </summary>
        ///     ''' <remarks></remarks>

        public Nullable<int> Avtalenummer { get; set; }

        ///// <summary>
        /////     ''' NB!!: Benyttes kun av og er kun implementert for integrasjon mot Ordre
        /////     ''' Har ellers ingen verdi satt.
        /////     ''' </summary>
        /////     ''' <remarks></remarks>

        //public DateTime SistOppdatert { get; set; }

        ///// <summary>
        /////     ''' NB!!: Benyttes kun av og er kun implementert for integrasjon mot Ordre.
        /////     ''' Har ellers ingen verdi satt.
        /////     ''' </summary>
        /////     ''' <remarks></remarks>

        //public string SistEndretAv { get; set; }


        //public long AntallWhenLastSaved { get; set; }


        public int Weight { get; set; }


        public PumaEnum.DistributionType DistributionType { get; set; }


        public DateTime DistributionDate { get; set; }


        public bool IsBasis { get; set; }


        public int BasedOn { get; set; }


        //public string BasedOnName { get; set; }


        public int WasBasedOn { get; set; }


        //public string WasBasedOnName { get; set; }


        public bool AllowDouble { get; set; }
        public long Antall
        {
            get;
            //{
            //    return CalculateAntall();
            //}
            set;
            //{
            //    _Antall = CalculateAntall();
            //}
        }
        private double _thickness;
        public double Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
            }
        }

        [JsonIgnore]
        public string userName { get; set; }

        public List<Puma.Shared.Utvalg> MemberUtvalgs
        {
            get; set;

        }

        public List<Puma.Shared.UtvalgList> MemberLists
        {
            get; set;

        }
    }


}
