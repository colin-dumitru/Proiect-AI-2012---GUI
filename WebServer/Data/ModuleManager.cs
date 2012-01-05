using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using WebServer.Models;

namespace WebServer.Data
{
    // numele clasei se poate schimba
    // asta mi-a venit pe moment
    public class ModuleManager
    {
        public IDocumentEntityManager EntityManager { get; set; }


        /// <summary>
        /// Valideaza un document. Daca statusul sau este gol, atunci 
        /// se porneste parsarea si apelarea celorlalte moduule.
        /// </summary>
        /// <param name="doc">Documentul ce trebuie validat</param>
        /// <returns>Fisierul XML daca exista. Null daca documentul inca nu a fost creat.</returns>
        public String ValidateDocumentOutput(DocumentOutput doc)
        {
            if (this.EntityManager == null)
                return null;

            switch (doc.Status) {
                    /*daca fisierul a fost creat, il intoarcems*/
                case DocumentManager.StatusOK:
                    return doc.Document;
                    /*daca documentul e in curs de parsare, nu facem nimic*/
                case DocumentManager.StatusParsing:
                    return null;
                    /*daca documentul este gol, pornim workerul ce se ocupa de apelarea modulelor*/
                case DocumentManager.StatusEmpty:
                    this._StartWorker(doc);
                    return null;
                default:
                    return null;
            }
        }

        private void _StartWorker(DocumentOutput doc) {
            ModuleWorker worker = new ModuleWorker(this.EntityManager, doc);

            /*cream un thread nou*/
            Thread thread = new Thread(new ThreadStart(worker.Run));
            thread.Start();

        }
    }
}
