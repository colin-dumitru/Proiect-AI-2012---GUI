using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Data;
using WebServer.ModelView;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WebServer.Controllers
{
    public class GetController : Controller
    {
        const String ROOT = @"C:\lib";

        private static Dictionary<int, Process> _serverQA = new Dictionary<int, Process>();   

        private IDocumentManager _documentManager = null;
        private IDocumentEntityManager _entityManager = null;
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        public GetController()
        {
            this._entityManager = new DocumentEntityManager();
            this._documentManager = new DocumentManager()
            {
                EntityManager = this._entityManager
            };

        }

        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        /// <summary>
        /// Actiunea ce intoarce un fisier XML, avand ca date timeline-ul documenutlui impreuna cu
        /// relatiile dintre personaje / relatii etc.
        /// </summary>		
        /// <param name="id">Id-ul documentului.</param>
        public ActionResult TimeLine(int? id)
        {
            /*verificam daca a fost specificat documentul*/
            if (id == null)
                return View("Index", "Home");


            /*modelul ce il intorcem*/
            DocumentResponse ret = new DocumentResponse();
            /*documentul propriuzis*/
            DocumentResponse doc = new DocumentResponse();

            /*luam documentul cautat*/
            try
            {
                doc.Content = this._documentManager.GetDocument(id ?? 0, DocumentManager.TypeTimeline);
                doc.State = (doc.Content != null) ? DocumentResponse.Status.Finished : DocumentResponse.Status.Parsing;
            }
            catch (DocumentException)
            {
                doc.State = DocumentResponse.Status.NotFound;
            }

            return new XmlResult(doc);
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Actiunea ce intoarce un fisier XML, avand ca date rezumatul documentului impreuna cu alte
        /// informatii generate asupra documentului.
        /// </summary>
        /// <param name="id">Id-ul documentului.</param>
        public ActionResult Summary(int? id)
        {
            /*verificam daca a fost specificat documentul*/
            if (id == null)
                return View("Index", "Home");

            /*documentul propriuzis*/
            DocumentResponse doc = new DocumentResponse();

            /*luam documentul cautat*/
            try
            {
                doc.Content = this._documentManager.GetDocument(id ?? 0, DocumentManager.TypeSummary);
                doc.State = (doc.Content != null) ? DocumentResponse.Status.Finished : DocumentResponse.Status.Parsing;
            }
            catch (DocumentException)
            {
                doc.State = DocumentResponse.Status.NotFound;
            }

            return new XmlResult(doc);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Actiunea ce intoarce un fisier XML, avand ca date un raspuns la intrebarea primita ca
        /// parametru.
        /// </summary>
        /// <param name="id">Id-ul documentului.</param>
        /// <param name="question">Intrebarea careia trebuie sa ii gasim raspunsul.</param>
        public ActionResult Answer(int? id, String question)
        {
            //this._StartServer(id ?? -1);
            

            return Json(new AnswerModelView() { Answer=this._Ask(id ?? -1, question) });
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private String _Ask(int id, String question) {
            String path = Directory.GetCurrentDirectory() + "\\" + id.ToString();

            using (Stream stream = new FileStream(path + "\\in.txt", FileMode.Create, FileAccess.Write)) {
                byte[] buffer = Encoding.UTF8.GetBytes(question);
                stream.Write(buffer, 0, buffer.Length);
            }

            /*conf*/
            String args = " -jar modulQA.jar ask \"" + path + "\\in.txt\" \"" + path + "\\out.txt\"";

            Process confProcess = Process.Start(new ProcessStartInfo("java", args) {
                WorkingDirectory = ROOT + "\\qa\\clientQA"
            });

            confProcess.WaitForExit();

            using (Stream stream = new FileStream(path + "\\out.txt", FileMode.Open, FileAccess.Read)) {
                return new StreamReader(stream).ReadToEnd();
            }

            //return "No answer";
            
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private void _StartServer(int id) {
            if(_serverQA.ContainsKey(id))
                return;

            String path = Directory.GetCurrentDirectory() + "\\" + id.ToString();

            /*daca nu exista resutatele xml, le cream*/
            this._documentManager.GetDocument(id, DocumentManager.TypeSummary);
            this._documentManager.GetDocument(id, DocumentManager.TypeTimeline);

            /*pornim procesul*/
            String args = " -jar JavaAIParser.jar";

            Process infoProcess = Process.Start(new ProcessStartInfo("java", args) {
                WorkingDirectory = ROOT + "\\qa\\serverQA"
            });

            _serverQA[id] = infoProcess;

            /*conf*/
            args = " -jar modulQA.jar book \"" +
                path + "\\info.xml\" \"" +
                path + "\\summ.xml\" \"" + 
                path + "\\sit.xml\"";

            Process confProcess = Process.Start(new ProcessStartInfo("java", args) {
                WorkingDirectory = ROOT + "\\qa\\clientQA"
            });
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

    }
}
