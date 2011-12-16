using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.ModelView;

namespace WebServer.Controllers {
    [HandleError]
    public class HomeController : Controller {

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
	/// <summary>
	/// Action-ul paginii principale (ce incarca fisierul ce trebuie parsat).
	/// </summary>
        public ActionResult Index() {
            return View(new DocumentModelView { DocumentId = 12354 });
        }
        
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Action-ul paginii ce afiseaza rezumatul impreuna cu alte informatii generale pe tot
        /// documentul.
        /// </summary>
        /// <param name="id">Id-ul documentului ce trebuie afisat</param>
        public ActionResult Summary(int? id) {
            /*nu este specificat id-ul documentului*/
            if (id == null)
                return View("Index");

            return View(new DocumentModelView { DocumentId = id ?? -1 });
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Action-ul paginii ce afisea timeline-ul documentului impreuna cu relatiile dintre
        /// personaje / locatii etc.
        /// </summary>
        /// <param name="id">Id-ul documentului ce trebui afisat</param>
        public ActionResult TimeLine(int? id) {
            /*nu este specificat id-ul documentului*/
            if (id == null)
                return View("Index");

            return View(new DocumentModelView { DocumentId = id ?? -1 });
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        ///  Action-ul paginii ce afiseaza dialogul cu intrebare-raspuns.
        /// </summary>
        /// <param name="id">Id-ul documentului ce trebuie interogat dupa intrebare.</param>
        public ActionResult Interactivity(int? id) {
            /*nu este specificat id-ul documentului*/
            if (id == null)
                return View("Index");

            return View(new DocumentModelView { DocumentId = id ?? -1 });
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------


    }
}
