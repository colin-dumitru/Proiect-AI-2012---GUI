using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebServer.Data
{

    public class DocumentManager : IDocumentManager
    {
        public int StoreDocument(System.IO.Stream fileStream)
        {
            // test
            throw new NotImplementedException();
        }

        public System.IO.Stream GetDocument(int id)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetDocument(int id, string type)
        {
            throw new NotImplementedException();
        }
    }
}