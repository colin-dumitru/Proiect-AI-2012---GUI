using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebServer.Controllers
{
    public class GetController : Controller
    {

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
		/// <summary>
		/// Actiunea ce intoarce un fisier XML, avand ca date timeline-ul documenutlui impreuna cu
        /// relatiile dintre personaje / relatii etc.
		/// </summary>		
        /// <param name="id">Id-ul documentului.</param>
        public ActionResult TimeLine(int? id)
        {
            return View();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Actiunea ce intoarce un fisier XML, avand ca date rezumatul documentului impreuna cu alte
        /// informatii generate asupra documentului.
        /// </summary>
        /// <param name="id">Id-ul documentului.</param>
        public ActionResult Summary(int? id) {
            return View();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Actiunea ce intoarce un fisier XML, avand ca date un raspuns la intrebarea primita ca
        /// parametru.
        /// </summary>
        /// <param name="id">Id-ul documentului.</param>
        /// <param name="question">Intrebarea careia trebuie sa ii gasim raspunsul.</param>
        public ActionResult Answer(int? id, String question) {
            return View();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

    }
}
