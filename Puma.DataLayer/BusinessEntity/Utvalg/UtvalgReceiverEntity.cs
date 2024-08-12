using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Utvalg
{
    public class UtvalgReceiverEntity
    {

        public int ReceiverId
        {
            get;set;
        }


        public int Selected
        {
            get;set;
        }

        public long UtvalgId { get; set; }
    }
}
