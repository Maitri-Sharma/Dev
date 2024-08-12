using DataAccessAPI.HandleRequest.Response.Calendar;
using MediatR;
using System;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Calendar
{
    public class RequestGetRuteinfo : IRequest<ResponseGetRuteinfo>
    {
        private List<long> _RuteIDs = new List<long>();

        public List<long> RuteIDs
        {
            get
            {
                return _RuteIDs;
            }

            set
            {
                _RuteIDs = value;
            }
        }

        private int m_Id;

        public int Id
        {
            get
            {
                return m_Id;
            }

            set
            {
                m_Id = value;
            }
        }

        private string m_Type;

        public string Type
        {
            get
            {
                return m_Type;
            }

            set
            {
                m_Type = value;
            }
        }

        private int m_Vekt;

        public int Vekt
        {
            get
            {
                return m_Vekt;
            }

            set
            {
                m_Vekt = value;
            }
        }

        private string m_Distribusjonstype;

        public string Distribusjonstype
        {
            get
            {
                return m_Distribusjonstype;
            }

            set
            {
                m_Distribusjonstype = value;
            }
        }

        private DateTime m_ValgtDato;

        public DateTime ValgtDato
        {
            get
            {
                return m_ValgtDato;
            }

            set
            {
                m_ValgtDato = value;
            }
        }

        private double m_Thickness;

        public double Thickness
        {
            get
            {
                return m_Thickness;
            }

            set
            {
                m_Thickness = value;
            }
        }
    }
}