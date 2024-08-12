using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgReceiverList : List<UtvalgReceiver>
    {
        public UtvalgReceiverList() : base()
        {
        }

        public UtvalgReceiverList(UtvalgReceiver receiver) : base()
        {
            this.Add(receiver);
        }

        public bool ContainsReceiver(PumaEnum.ReceiverType recType)
        {
            foreach (UtvalgReceiver ur in this)
            {
                if (ur.ReceiverId == recType)
                    return true;
            }
            return false;
        }

        public void IncludeReceiver(PumaEnum.ReceiverType recType, bool include)
        {
            if (include)
            {
                if (!ContainsReceiver(recType))
                    this.Add(new UtvalgReceiver(recType));
            }
            else
                this.RemoveReceiver(recType);
            _ReceiversChanged = true;
        }

        private void RemoveReceiver(PumaEnum.ReceiverType recType)
        {
            foreach (UtvalgReceiver ur in new ArrayList(this))
            {
                if (ur.ReceiverId == recType)
                    this.Remove(ur);
            }
        }

        /// <summary>
        ///     ''' Check if receiverList only include a single receivertype
        ///     ''' </summary>
        ///     ''' <param name="receiverType"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public bool HasOnlyReceiverType(PumaEnum.ReceiverType receiverType)
        {
            if ((this.Count == 1 & this.ContainsReceiver(receiverType)))
                return true;
            return false;
        }

        // Only used in Reolutforsker to check if PopulateTree should be called
        private bool _ReceiversChanged = false;
        public bool ReceiversChanged
        {
            get
            {
                return _ReceiversChanged;
            }
            set
            {
                _ReceiversChanged = value;
            }
        }
    }
}
