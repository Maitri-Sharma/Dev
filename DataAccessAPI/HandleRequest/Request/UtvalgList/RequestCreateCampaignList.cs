using MediatR;
using System;
using Puma.Shared;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestCreateCampaignList : IRequest<ResponseCreateCampaignList>
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




        public string Name { get; set; }





        public int BasedOn { get; set; }


       

        public string userName { get; set; }

        public double Antall { get; set; }

    }
}
