using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{

    public class UtvalgReceiver
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

        public UtvalgReceiver()
        {
        }

        public UtvalgReceiver(PumaEnum.ReceiverType recType)
        {
            this.ReceiverId = recType;
            this.Selected = true;
        }
    }
}
