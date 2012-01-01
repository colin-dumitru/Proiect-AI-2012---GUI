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
        public const String TypeSummary = "summary";
        public const String TypeTimeline = "timeline";

        public const int StatusOK = 1;
        public const int StatusEmpty = 0;
        public const int StatusParsing = 2;

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
            this.EntityManager.AddDocumentOutput(doc.Id, TypeMain, StatusEmpty, fileStream);
            /*si celalte fisiere goale*/
            this.EntityManager.AddDocumentOutput(doc.Id, TypeSummary, StatusEmpty, "");
            this.EntityManager.AddDocumentOutput(doc.Id, TypeTimeline, StatusEmpty, "");


            //returnam id-ul
            return doc.Id;
        }

        public String GetDocument(int id)
        {
            return null;
           
        }

        public String GetDocument(int id, string type)
        {
            DocumentOutput res = null;

            try
            {
                 res = this.EntityManager.GetDocumentOutput(id, type);
                 if (res == null)
                     throw new EntityManagerException();
            }
            catch (EntityManagerException)
            {
                throw new DocumentException("Documentul nu a fost gasit");
            }

            if (res.Status == StatusOK)
                return res.Document;
            else
                return null;
        }

        
    }
}