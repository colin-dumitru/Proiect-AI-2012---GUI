﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.ModelView;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class LoadController : Controller
    {
        private IDocumentManager _documentManager = null;
        private IDocumentEntityManager _entityManager = null;

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public LoadController() {
            /*initializam entity manager-ul*/
            this._entityManager = new DocumentEntityManager();
            /*TODO initializam document manager*/
            this._documentManager = new DocumentManager() {
                EntityManager = this._entityManager
            };
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Actiunea ce incarca un fisier ce trebuie parsat.
        /// </summary>
        /// <param name="file">Fisierul primit de la client.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file) {
            /*id-ul fisierul nou uploadat*/
            int id = 0;

            /*trimitem documentul pentru a fi parsat*/
            if(this._documentManager != null)
                id = this._documentManager.StoreDocument(file.FileName, file.InputStream);

            return RedirectToAction("Summary", "Home", new { id = id });
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        ///  Action-ul paginii ce incarca un document deja parsat.
        /// </summary>
        /// <param name="id">Id-ul documentului ce trebuie incarcat.</param>    
        public ActionResult Load(int? id) {
            /*nu este specificat id-ul documentului*/
            if (id == null)
                return View("Index");

            return RedirectToAction("Summary", "Home", new { id = id ?? -1});
        }

    }
}
