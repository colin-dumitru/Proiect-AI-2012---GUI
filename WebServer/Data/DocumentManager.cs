using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

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
        public const String MainDocument = "main";

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

        public int StoreDocument(Stream fileStream)
        {
            if (this._config == null)
                throw new DocumentException("Configuratie inexistenta");

            /*id-ul il generam unic, incrementand 1 la ultimul id generat*/
            int id = this._config.UniqueId;         
          
            string newFolder = Environment.CurrentDirectory + "\\" + id.ToString();

            // creem directorul
            Directory.CreateDirectory(newFolder);

            using(Stream stream = File.Open(newFolder + "\\" + MainDocument, 
                FileMode.OpenOrCreate, FileAccess.Write)) {
                 /*copiem datele din streamul vechi in documentul principal -- numele va fie mereu acelasi*/
                 fileStream.CopyTo(stream);
            }

            //returnam id-ul
            return id;
        }

        public Stream GetDocument(int id)
        {
            // verificam mai intai daca exista fisierul cu id-ul respectiv
            if (!Directory.Exists(Environment.CurrentDirectory + "\\" + id.ToString()))
            {
                throw new DocumentException("Nu exista un document cu id-ul respectiv");
            }

            /*deschidem un stream catre documentul principal*/
            return File.OpenRead(Environment.CurrentDirectory + "\\" + id.ToString() +
                "\\" + MainDocument);

           
        }

        public System.IO.Stream GetDocument(int id, string type)
        {
            throw new NotImplementedException();
        }
    }
}