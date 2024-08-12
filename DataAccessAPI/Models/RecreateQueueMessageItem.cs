using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessAPI.Models
{
    public class RecreateQueueMessageItem
    {
        private char _type;

        public char Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        private int _id;

        public long Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = (int)value;
            }
        }

        private string _email;

        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
            }
        }

        public static explicit operator RecreateQueueMessageItem(string v)
        {
            throw new NotImplementedException();
        }
    }
}
