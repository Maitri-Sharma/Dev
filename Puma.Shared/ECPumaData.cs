using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public partial class ECPumaData : UtvalgBase
    {
        public ECPumaData()
        {
        }

        public ECPumaData(int id, int parentId, PumaEnum.UtvalgsTypeKode utvalgsType, string navn, int antall, DateTime sistOppdatert, string previewURL, string sistEndretAv)
        {
            base.UtvalgId = id;
            base.ParentId = parentId;
            base.Type = utvalgsType;
            base.Navn = navn;
            
            this.antall = antall;
            this.previewURL = previewURL;
            this.sistOppdatert = sistOppdatert;
            this.sistEndretAv = sistEndretAv;
        }


        private int antall;
        public int Antall
        {
            get { return this.antall; }
            set { this.antall = value; }
        }
        
        private string previewURL;
        public string PreviewURL
        {
            get { return this.previewURL; }
            set {this.previewURL = value; }
        }

        private DateTime sistOppdatert;
        public DateTime SistOppdatert
        {
            get { return sistOppdatert; }
            set { sistOppdatert = value; }
        }

        private String sistEndretAv;
        public String SistEndretAv
        {
            get { return sistEndretAv; }
            set { sistEndretAv = value; }
        }
    }
}