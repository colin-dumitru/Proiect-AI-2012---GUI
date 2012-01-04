using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ApelareCelelalteModule
{
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
