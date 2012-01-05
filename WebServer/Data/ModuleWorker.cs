using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WebServer.Models;

namespace WebServer.Data
{
    // Thread-ul in care se va trata cererea
    public class ModuleWorker
    {
        private IDocumentEntityManager _em = null;
        private DocumentOutput _doc = null;
        
        public ModuleWorker(IDocumentEntityManager em, DocumentOutput doc)
        {
            this._em = em;
            this._doc = doc;
        }

        public void Run()
        {
            if(this._em == null || this._doc == null)
                return;

            /*schimbam statusul initial sa spunem ca documentul se parseaza*/
            this._doc.Status = DocumentManager.StatusParsing;
            this._em.Save();

            /*rezultatul apelarii*/
            Boolean res = false;

            switch (this._doc.Type)
	        {
                case DocumentManager.TypeSummary:
                    res = this._CreateSummaryDocument();
                    break;
		        case DocumentManager.TypeTimeline:
                    res = this._CreateTimelineDocument();
                    break;
	        }

            if (res) {
                /*daca a reusit parsarea facem update la baza de date*/
                this._doc.Status = DocumentManager.StatusOK;
                this._em.Save();
            } else {
                /*mai bagam o fisa*/
                this._doc.Status = DocumentManager.StatusEmpty;
                this._em.Save();
            }
        }

        private Boolean _CreateSummaryDocument() {
            /*apeleaza modulele de sumarizare extragere de informatii si creaza
             documentul xml*/

            /*daca a reusit apelarea modulelor*/
            return false;
        }

        private Boolean _CreateTimelineDocument() {
            /*apeleaza modulele de situatii si timp si ceraza documentul xml*/

            /*daca a reusit apelarea modulelor*/
            return false;
        }
    }
}
