using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebServer.Data {
    public interface IDocumentManager {
        /// <summary>
        /// Trimite documentul spre a fi parsat, si salveaza rezultatele cu un id unic.
        /// </summary>
        /// <param name="fileStream">Fisierul ce trebuie parsat.</param>
        /// <returns>Id-ul unic ce va fi folosit pentru a lua ulterior datele.</returns>
        int StoreDocument(Stream fileStream);

        /// <summary>
        /// Intoarce documentul original.
        /// </summary>
        /// <param name="id">Id-ul documentului ce trebuie intors.</param>
        /// <returns>Un stream ce arata spre documentul original, sau null daca nu a fost gasit</returns>
        Stream GetDocument(int id);

        /// <summary>
        /// Intoarce unul din documentele rezultante din parsarea celui original.
        /// </summary>
        /// <param name="id">id-ul documentului original.</param>
        /// <param name="type">Tipul documentului ce trebuie intors (eg: rezumat, timeline etc.)</param>
        /// <returns>Un stream ce arata spre documentul dorit, sau null daca nu a fost gasit </returns>
        Stream GetDocument(int id, String type);

    }
}