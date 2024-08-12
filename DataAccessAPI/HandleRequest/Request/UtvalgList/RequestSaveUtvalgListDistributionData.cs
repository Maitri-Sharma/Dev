using MediatR;
using System;
using Puma.Shared;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSaveUtvalgListDistributionData : IRequest<bool>
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


        public List<long> ruteId { get; set; }
    }
}
