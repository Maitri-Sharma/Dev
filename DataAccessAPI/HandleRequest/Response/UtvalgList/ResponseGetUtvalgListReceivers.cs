using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.UtvalgList
{
    public class ResponseGetUtvalgListReceivers
    {
        private PumaEnum.ReceiverType _ReceiverId;

        public PumaEnum.ReceiverType ReceiverId
        {
            get
            {
                return _ReceiverId;
            }
            set
            {
                _ReceiverId = value;
            }
        }

        private bool _Selected;

        public bool Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                _Selected = value;
            }
        }

        //public UtvalgReceiverList()
        //{
        //}

        //public UtvalgReceiverList(PumaEnum.ReceiverType recType)
        //{
        //    this.ReceiverId = recType;
        //    this.Selected = true;
        //}
    }
}
