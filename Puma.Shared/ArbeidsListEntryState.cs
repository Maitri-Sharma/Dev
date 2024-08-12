using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class ArbeidsListEntryState
    {
        private int _id = -1;
        private PumaEnum.ListType _type = PumaEnum.ListType.Unsettled;
        private string _userId = null;
        private bool _active = false;

        public ArbeidsListEntryState(int id, PumaEnum.ListType type, string userId, bool active)
        {
            if (type == PumaEnum.ListType.Unsettled)
                throw new Exception("type " + type.ToString() + " is not valid for sub ArbeidsListEntryState");
            if (userId == null)
                throw new Exception("userId can not be null in  ArbeidsListEntryState!");

            this._id = id;
            this._type = type;
            this._userId = userId;
            this._active = active;
        }

        public int Id
        {
            get
            {
                return this._id;
            }
        }

        public PumaEnum.ListType Type
        {
            get
            {
                return this._type;
            }
        }

        public string UserId
        {
            get
            {
                return this._userId;
            }
        }

        public bool Active
        {
            get
            {
                return this._active;
            }
        }

        public char GetTypeCharRepresentation()
        {
            switch (this.Type)
            {
                case PumaEnum.ListType.Utvalg:
                    {
                        return 'U';
                    }

                case PumaEnum.ListType.UtvalgList:
                    {
                        return 'L';
                    }

                default:
                    {
                        throw new Exception("Type " + this.Type.ToString() + " is not valid for function GetTypeCharRepresentation");
                    }
            }
        }

        public static PumaEnum.ListType GetTypeValueFromChar(char typeChar)
        {
            if (typeChar == 'U')
                return PumaEnum.ListType.Utvalg;
            else if (typeChar == 'L')
                return PumaEnum.ListType.UtvalgList;
            else
                throw new Exception("typechar value " + typeChar + " is not supported for function GetTypeValueFromChar");
        }

        public static bool GetActiveValueFromChar(char activeChar)
        {
            if (activeChar == 'T')
                return true;
            else if (activeChar == 'F')
                return false;
            else
                throw new Exception("activeChar value " + activeChar + " is not supported for function GetActiveValueFromChar");
        }


        //public string GetActiveBoolCharRepresentation()
        //{
        //    if (this.Active)
        //        return 'T';
        //    else
        //        return 'F';
        //}
    }

}
