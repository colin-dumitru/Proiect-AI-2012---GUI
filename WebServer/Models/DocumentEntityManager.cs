using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace WebServer.Models {
    [Serializable]
    public class EntityManagerException : Exception {
        public EntityManagerException() { }
        public EntityManagerException(string message) : base(message) { }
        public EntityManagerException(string message, Exception inner) : base(message, inner) { }
        protected EntityManagerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    public class DocumentEntityManager : IDocumentEntityManager{
        private DocumentDataModelContainer _dbContainer = null;

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public DocumentEntityManager() {
            this._dbContainer = new DocumentDataModelContainer();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        ~DocumentEntityManager ()
        {
            if (this._dbContainer != null) {
                this._dbContainer.SaveChanges();
                this._dbContainer.Dispose();
            }                                           
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
				
        public Document CreateDocument(string documenName) {
            if (this._dbContainer == null)
                throw new EntityManagerException("Nu suntem conectati la baza de date!");

            /*cream un document nou*/
            Document doc = new Document() {
                Name = documenName
            };

            /*adaugam obiectul in baza de date*/
            this._dbContainer.Documents.AddObject(doc);
            this._dbContainer.SaveChanges();

            return doc;
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        public Document GetDocument(int documentid) {
            throw new NotImplementedException();

        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        public DocumentOutput AddDocumentOutput(int documentId, string type, System.IO.Stream file) {
            if (this._dbContainer == null)
                throw new EntityManagerException("Nu suntem conectati la baza de date!");

            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, (int)file.Length);

            return this.AddDocumentOutput(documentId, type,
                Encoding.UTF8.GetString(buffer, 0, buffer.Length));                        
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        public DocumentOutput AddDocumentOutput(int documentId, string type, string file) {
            if (this._dbContainer == null)
                throw new EntityManagerException("Nu suntem conectati la baza de date!");

            /*verificam daca exista un document cu id-ul respectiv*/
            List<Document> rezDoc = (from d in this._dbContainer.Documents
                                        where d.Id == documentId
                                        select d).ToList();     
            
            if(rezDoc.Count != 1)
                throw new EntityManagerException("Nu exista un document cu id-ul specificat.");      


            /*verificam daca mai sunt alte fisiere pentru acelasi document id cu tipul specificat*/
            List<DocumentOutput> rezOut = (from d in this._dbContainer.DocumentOutputs
                                        where d.DocumentId == documentId && d.Type == type
                                        select d).ToList();                                                    
            if (rezOut.Count > 0)
                throw new EntityManagerException("Mai exista si alte fisiere cu acelasi tip."); 

            DocumentOutput ret = new DocumentOutput() {
                DocumentId = documentId,
                Document = file,
                Type = type
            };

            /*adaugam fisierul in baza noastra de daze*/
            this._dbContainer.DocumentOutputs.AddObject(ret);
            this._dbContainer.SaveChanges();

            return ret;
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        public DocumentOutput GetDocumentOutput(int documentId, string type) {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        public void RemoveDocument(int documentId) {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
    }
}