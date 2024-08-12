using DataAccessAPI.HandleRequest.Response.Calendar;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Calendar
{
    public class RequestGetRestcapacity : IRequest<ResponseGetRestcapacity>
    {
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

        private DateTime m_StartDato;
        public DateTime StartDato
        {
            get
            {
                return m_StartDato;
            }
            set
            {
                m_StartDato = value;
            }
        }

        private DateTime m_SluttDato;
        public DateTime SluttDato
        {
            get
            {
                return m_SluttDato;
            }
            set
            {
                m_SluttDato = value;
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
