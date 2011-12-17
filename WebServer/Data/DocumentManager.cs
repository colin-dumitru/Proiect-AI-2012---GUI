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
        public int StoreDocument(Stream fileStream)
        {
            // valoarea de return
            int id = 0;

            //verficam mai intai daca exista in directorul curent 
            // vreun folder care are nume de tip ID (adica un numar)
            // daca nu exista, vom stoca fisierul primit la intrare intr-un folder cu numele "0"
            // in caz contrar, numele noului folderului va fi cel mai mic numar intreg
            // care nu reprezinta numele nici unui alt subfolder
            // Ex: daca folderul curent are subfolderele 0 1 2 3, atunci numele noului subfolder va fi 4
            // Daca fodlerul are subfolderele 0 1 3, atunci numele noului folder va fi 2

            // vom obtine mai intai toate subfolderele
            DirectoryInfo[] foldereCopil = new DirectoryInfo(Environment.CurrentDirectory).GetDirectories();
            while (true)
            {
                // gata ne va spune daca iesim sau nu din bucla infinita
                bool gata = true;

                // comparam numele fiecarui subfolder cu ID-ul curent (care porneste initial de la 0, si apoi tot creste)
                foreach (DirectoryInfo copil in foldereCopil)
                    // daca exista deja folderul cu numele id
                    // incrementam id
                    // gata va fi fals pt a nu iesi din bucla infinita inca
                    // si oprim foreach-ul, deoarece nu mai are sens sa cautam 
                    // pt. moment (deja stim ca exista un folder cu numele id)
                    if (copil.Name == id.ToString())
                    {
                        id++;
                        gata = false;
                        break;
                    }
                // daca gata a ramas true, putem iesi din bucla infinita
                // pt. ca am gasit ceea ce cautam
                if (gata)
                    break;
            }
            // newFolder va fi numele folderului pe care il vom crea
            // calea sa va fi calea directorului curent concatenat cu numele id-ului
            string newFolder = Environment.CurrentDirectory + "\\" + id.ToString();

            // creem directorul
            Directory.CreateDirectory(newFolder);

            // avem nevoie de un FileStream deoarece dintr-un obiect de tip Stream
            // nu putem afla numele fisierului catre care "pointeaza" Stream-ul
            FileStream fStream = (FileStream)fileStream;

            // obtinem numele fisierului catre care "pointeaza" Stream-ul
            string numeFisier = new FileInfo(fStream.Name).Name;

            // copiem fisierul in noul folder
            fileStream.CopyTo(new FileStream(newFolder + "\\" + numeFisier, FileMode.Create));

            // inchidem FileStream-ul
            fStream.Close();

            //returnam id-ul
            return id;
        }

        public Stream GetDocument(int id)
        {
            // verificam mai intai daca exista fisierul cu id-ul respectiv

            // obtinem toate folderele din directorul curent
            DirectoryInfo[] foldereCopil = new DirectoryInfo(Environment.CurrentDirectory).GetDirectories();

            // gasit ne va spune daca l-am gasit
            bool gasit = false;

            // comparam numele fiecarui subfolder cu id
            foreach (DirectoryInfo copil in foldereCopil)
                if (copil.Name == id.ToString())
                {
                    // daca l-am gasit, il setam pe gasit pe true
                    gasit = true;

                    // si iesim din bucla
                    break;
                }
            if (gasit)
            // daca s-a gasit
            {
                // obtinem calea catre fisierul respectiv, care va fi
                // (teoretic) singurul din folderul respectiv, 
                // deci cel de indice 0 din lista de fisiere a folderului
                string fisier = new DirectoryInfo(id.ToString()).GetFiles()[0].FullName;

                // creem un FileStream car fisier
                FileStream stream = new FileStream(fisier, FileMode.Open);

                // si returnam stream-ul respectiv
                return stream;
            }
            return null;
        }

        public System.IO.Stream GetDocument(int id, string type)
        {
            throw new NotImplementedException();
        }
    }
}