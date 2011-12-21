using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using WebServer.Models;

namespace WebServer.Data
{

    [Serializable]
    class DocumentManagerConfig
    {
        private int _lastid = 0;


        /*numarul folosit la generarea id-urilor unice*/
        public int UniqueId
        {
            get
            {
                return (this._lastid++);
            }
        }
    }

    [Serializable]
    public class DocumentException : Exception
    {
        public DocumentException() { }
        public DocumentException(string message) : base(message) { }
        public DocumentException(string message, Exception inner) : base(message, inner) { }
        protected DocumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    

    public class DocumentManager : IDocumentManager
    { 
        public const String ConfigFile = "config";
        public const String TypeMain = "main";

        /*folosit pentru a adauga documente in baza de date*/
        public IDocumentEntityManager EntityManager { get; set; }

        /*configuratia managerului*/
        private DocumentManagerConfig _config = null;        

        public DocumentManager()
        {
            /*in constructor deserializam configuratia*/
            Stream stream = null;

            try
            {
                stream = File.OpenRead(ConfigFile);
                BinaryFormatter bf = new BinaryFormatter();
                this._config = bf.Deserialize(stream) as DocumentManagerConfig;

                if (this._config == null)
                    throw new Exception();

            }
            catch (Exception)
            {
                this._config = new DocumentManagerConfig();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        ~DocumentManager()
        {
            /*in constructor deserializam configuratia*/
            Stream stream = null;

            try
            {
                stream = File.OpenWrite(ConfigFile);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, this._config);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public int StoreDocument(String fileName, Stream fileStream)
        {
            if (this._config == null || this.EntityManager == null)
                throw new DocumentException("Configuratie inexistenta");

            /*adaugam un document nou ce va contine fisierele propriuzise*/
            Document doc = this.EntityManager.CreateDocument(fileName);

            /*adaugam fisierul initial*/
            this.EntityManager.AddDocumentOutput(doc.Id, TypeMain, fileStream);

            //returnam id-ul
            return doc.Id;
        }

        public String GetDocument(int id)
        {
            return null;
           
        }

        public String GetDocument(int id, string type)
        {
            throw new NotImplementedException();
        }

        
    }
}