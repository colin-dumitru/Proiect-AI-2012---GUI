using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ApelareCelelalteModule
{
    // Thread-ul in care se va trata cererea
    public class ClientThread
    {
        private Stream fisierXML;
        public ClientThread(Stream fisierXML)
        {
            this.fisierXML = fisierXML;
        }
        public void TrateazaCerere()
        {
            // se va trata cererea baza lui fisierXML
        }
    }
}
