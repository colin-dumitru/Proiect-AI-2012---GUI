using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace ApelareCelelalteModule
{
    public class ApelareModul
    {
        public int ObtineStatus(Stream fisierXML)
        {
            // se va cauta in baza de date fisierul
            // si se va returna statusul
            return 0;
        }

        public string ObtineTip(Stream fisierXML)
        {
            // se va cauta in baza de date
            // si se va returna tipul
            return "";
        }

        public Stream ObtineOutputul(Stream fisierXML)
        {
            // se cauta in baza de date
            return null;
        }

        public Stream PrimesteCerere(Stream fisierXML)
        {
            int status = ObtineStatus(fisierXML);
            if (status == 1) // verificam mai intai daca statusul este ok
                             // adica, daca s-a terminat parsarea si poate intoarce rezultatul
            {
                // se cauta in baza de date output.xml si se intoarce
                return ObtineOutputul(fisierXML);
            }
            // else
            string tip = ObtineTip(fisierXML);

            if (tip == "summary")
            {
                // se seteaza statusul la 2 (parsing)
                ClientThread ct = new ClientThread(fisierXML);
                new Thread(new ThreadStart(ct.TrateazaCerere)).Start();
                return null;
            }
            return null;            
        }
    }
}
