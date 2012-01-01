using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace WebServer.Controllers
{
    public class XmlResult : ActionResult
    {
        private Object _model = null;


        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        public XmlResult(Object model)
        {
            if(model == null)
                throw new ArgumentNullException("The model cannot be null");

            this._model = model;
        }
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            var serializer = new XmlSerializer(_model.GetType());
            
            response.ContentType = "text/xml";
            serializer.Serialize(response.OutputStream, this._model);
        }
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
    }
}