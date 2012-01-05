using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WebServer.Models {
    public interface IDocumentEntityManager {
        /// <summary>
        /// Genereaza o inregistrare noua cu id unic pentru documente.
        /// </summary>
        /// <returns>Inregistrarea noua ce contine si id-ul unic</returns>
        Document CreateDocument(String documenName);

        /// <summary>
        /// Returneaza inregistrarea pentru documentul cu id-ul dat.
        /// </summary>
        /// <param name="documentid">Id-ul documentului ce trebuie intors</param>
        /// <returns>Documentul daca a fost gasit, null altfel.</returns>
        Document GetDocument(int documentid);

        /// <summary>
        /// Adauga un fisier nou pentru documentul cu id-ul documentId.
        /// </summary>
        /// <param name="documentId">Id-ul unic al documentului</param>
        /// <param name="type">Tipul documentului (eg: summary, timeline)</param>
        /// <param name="file">Un stream ce arata spre fisierul propriuzis</param>
        /// <param name="status">Statusul initialt al documentului.</param>
        DocumentOutput AddDocumentOutput(int documentId, String type, int status, Stream file);

        /// <summary>
        /// Adauga un fisier nou pentru documentul cu id-ul documentId.
        /// </summary>
        /// <param name="documentId">Id-ul unic al documentului</param>
        /// <param name="type">Tipul documentului (eg: summary, timeline)</param>
        /// <param name="file">Continutul fisierului</param>
        /// <param name="status">Statusul initialt al documentului.</param>
        DocumentOutput AddDocumentOutput(int documentId, String type, int status, String file);

        /// <summary>
        /// Intoarce un fisier din baza de date legat
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        DocumentOutput GetDocumentOutput(int documentId, String type);

        /// <summary>
        /// Sterge documentul din baza de date.
        /// </summary>
        /// <param name="documentId">Id-ul documentului ce trebuie sters</param>
        void RemoveDocument(int documentId);

        /// <summary>
        /// Salveaza orice modificari aduse bazei de date.
        /// </summary>
        void Save();
    }
}
