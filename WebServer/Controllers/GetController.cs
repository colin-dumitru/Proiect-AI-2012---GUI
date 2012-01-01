using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Data;

namespace WebServer.Controllers
{
    public class GetController : Controller
    {
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


            /*modelul ce il intorcem*/
            DocumentResponse ret = new DocumentResponse();
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
            return View();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

    }
}
