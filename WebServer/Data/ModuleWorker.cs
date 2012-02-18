using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WebServer.Models;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using NClassifier.Summarizer;
using System.Text.RegularExpressions;

namespace WebServer.Data {
    // Thread-ul in care se va trata cererea
    public class ModuleWorker {
        /*lene sa fac altfel acum*/
        const String ROOT = @"C:\lib";

        private IDocumentEntityManager _em = null;
        private DocumentOutput _doc = null;

        public ModuleWorker(IDocumentEntityManager em, DocumentOutput doc) {
            this._em = em;
            this._doc = doc;
        }

        public void Run() {
            if (this._em == null || this._doc == null)
                return;

            /*schimbam statusul initial sa spunem ca documentul se parseaza*/
            this._doc.Status = DocumentManager.StatusParsing;
            this._em.Save();

            String dir = this._doc.DocumentId.ToString();

            /*cream un folder temporar in care punem fisierele*/
            String path = Directory.CreateDirectory(dir).FullName;

            using (Stream mainStream = File.OpenWrite(dir + @"\main.txt")) {
                byte[] buffer = Encoding.UTF8.GetBytes(this._doc.Document);
                mainStream.Write(buffer, 0, buffer.Length);
            }

            /*rezultatul apelarii*/
            Boolean res = false;

            switch (this._doc.Type) {
                case DocumentManager.TypeSummary:
                    res = this._CreateSummaryDocument(path);
                    break;
                case DocumentManager.TypeTimeline:
                    res = this._CreateTimelineDocument(path);
                    break;
            }

            if (res) {
                /*daca a reusit parsarea facem update la baza de date*/
                this._doc.Status = DocumentManager.StatusOK;
            } else {
                /*mai bagam o fisa*/
                this._doc.Status = DocumentManager.StatusEmpty;
            }
            this._em.Save();
        }

        private Boolean _CreateSummaryDocument(string root) {
            try {

                /*extragem infomatiile*/
                DocumentOutput infoDoc = this._GetInformation(root);
                /*extragem resultatul*/
                DocumentOutput summaryDoc = this._GetSummary(root);

                /*le combinam*/
                DocumentOutput summary = this._CombineSummary(infoDoc, summaryDoc);

                /*daca a reusit apelarea modulelor*/
                return true;
            } catch {
                return false;
            }
        }

        private Boolean _CreateTimelineDocument(string root) {
            /*sunt nevoie si de situatii*/
            DocumentOutput infoDoc = this._GetInformation(root);
            /*cream documentul cu situatiile*/
            DocumentOutput sitDoc = this._GetSituations(root);



            /*daca a reusit apelarea modulelor*/
            return true;
        }

        private DocumentOutput _GetInformation(string root) {
            DocumentOutput doc = null;

            /*verificam daca este deja documentul in bd*/
            try {
                doc = this._em.GetDocumentOutput(this._doc.DocumentId, "information");
                /*ne asiguram ca este scris*/
                using (Stream stream = File.OpenWrite(root + "\\info.xml")) {
                    byte[] buffer = Encoding.UTF8.GetBytes(doc.Document);
                    stream.Write(buffer, 0, buffer.Length);
                }
            } catch { }

            if (doc != null)
                return doc;

            /*pornim procesul*/
            String args = " -Xms1500m -Xmx1500m -jar InformationExtraction.jar input=\"" +
                root + "\\main.txt\" " + "output=\"" + root + "\\info.xml\" " +
                " find genre characters locations dates relationships actions";

            Process infoProcess = Process.Start(new ProcessStartInfo("java", args) {
                    WorkingDirectory = ROOT + "\\info"
                });

            infoProcess.WaitForExit();

            /*citim fisierul si il punem in bd*/
            using (Stream stream = File.OpenRead(root + "\\info.xml")) {
                StreamReader sr = new StreamReader(stream);

                /*adaugam documentul in baza de date*/
                doc = this._em.AddDocumentOutput(this._doc.DocumentId, "information",
                    DocumentManager.StatusOK, sr.ReadToEnd());
            }

            return doc;
        }

        private DocumentOutput _GetSummary(string root) {
            DocumentOutput doc = null;

            /*verificam daca este deja documentul in bd*/
            try {
                doc = this._em.GetDocumentOutput(this._doc.DocumentId, "summ");
                /*ne asiguram ca este scris*/
                using (Stream stream = File.OpenWrite(root + "\\summ.xml")) {
                    byte[] buffer = Encoding.UTF8.GetBytes(doc.Document);
                    stream.Write(buffer, 0, buffer.Length);
                }
            } catch { }

            if (doc != null)
                return doc;

            String input = null;

            using (StreamReader stream = new StreamReader(root + "\\main.txt")) {
                input = stream.ReadToEnd();                
            }
            
            /*cream modulu de sumarizare*/
            /*ISummarizer summarizer = new SimpleSummarizer();
            String output = summarizer.Summarize(input, 40);*/

            String output = null;
            using (StreamReader info = new StreamReader(File.OpenRead(root + "\\info.xml"))) {
                XElement infoDoc = XElement.Parse(info.ReadToEnd());
                output = new Summarisation().ProcessXML(infoDoc);
            }

            if (output == null) return null;

            /*punem in baza de date rezultatul*/

            doc = this._em.AddDocumentOutput(this._doc.DocumentId, "summ",
                DocumentManager.StatusOK, output);

            /*scriem si in fisier*/
            using (Stream stream = File.OpenWrite(root + "\\summ.xml")) {
                byte[] buffer = Encoding.UTF8.GetBytes(output);
                stream.Write(buffer, 0, buffer.Length);
            }

            return doc;
        }

        private DocumentOutput _CombineSummary(DocumentOutput info, DocumentOutput summ) {
            XmlDocument root = new XmlDocument();
            XmlDocument infoDoc = new XmlDocument(); infoDoc.LoadXml(info.Document);

            var summary = root.CreateElement("summary"); root.AppendChild(summary);
            /*numele romanului*/
            var name = root.CreateElement("name"); summary.AppendChild(name);
            var nameText = root.CreateTextNode("Document title"); name.AppendChild(nameText);

            var extraction = root.CreateElement("extraction"); summary.AppendChild(extraction);
            /*tipul romanului*/
            var type = root.CreateElement("type"); extraction.AppendChild(type);
            foreach (XmlElement genre in infoDoc.GetElementsByTagName("genre")) {
                /*luam doar primul*/
                type.AppendChild(root.CreateTextNode(genre.GetAttribute("name")));
                break;
            }

            /*personajele*/
            var characters = root.CreateElement("characters"); extraction.AppendChild(characters);

            foreach (XmlElement ch in infoDoc.GetElementsByTagName("character")) {
                String cname = Regex.Replace(ch.InnerText, @"[^A-Za-z0-9 ]+", "").Trim();
                var che = root.CreateElement("character"); characters.AppendChild(che);
                che.SetAttribute("name", cname);
                if (ch.HasAttribute("main")) che.SetAttribute("type", "main");
                if (ch.HasAttribute("secondary")) che.SetAttribute("type", "secondary");
            }

            /*locatiile*/
            var locations = root.CreateElement("locations"); extraction.AppendChild(locations);

            foreach (XmlElement loc in infoDoc.GetElementsByTagName("location")) {
                String cloc = Regex.Replace(loc.InnerText, @"[^A-Za-z0-9 ]+", "");

                var loce = root.CreateElement("location"); locations.AppendChild(loce);
                loce.SetAttribute("name", cloc);
            }

            /*relatiile*/
            var relations = root.CreateElement("relations"); extraction.AppendChild(relations);

            foreach (XmlElement rel in infoDoc.GetElementsByTagName("relationship")) {
                String ent1 = Regex.Replace(rel.GetAttribute("entity1"), @"[^A-Za-z0-9 ]+", "").Trim();
                String ent2 = Regex.Replace(rel.GetAttribute("entity2"), @"[^A-Za-z0-9 ]+", "").Trim();
                String link = Regex.Replace(rel.GetAttribute("link"), @"[^A-Za-z0-9 ]+", "");

                var rele = root.CreateElement("relation"); relations.AppendChild(rele);
                rele.SetAttribute("character1", ent1);
                rele.SetAttribute("character2", ent2);
                rele.SetAttribute("verb", link);
            }

            /*rezumatul*/
            var summarisation = root.CreateElement("summarisation"); summary.AppendChild(summarisation);
            var summary2 = root.CreateElement("summary"); summary.AppendChild(summary2);

            summary2.InnerText = summ.Document;

            MemoryStream ms = new MemoryStream();
            root.Save(ms);
            return this._em.AddDocumentOutput(this._doc.DocumentId, "summary", DocumentManager.StatusOK, 
                Encoding.UTF8.GetString(ms.ToArray())) ;
        }

        private DocumentOutput _GetSituations(string root) {
            DocumentOutput doc = null;

            /*verificam daca este deja documentul in bd*/
            try {
                doc = this._em.GetDocumentOutput(this._doc.DocumentId, "situations");
                /*ne asiguram ca este scris*/
                using (Stream stream = File.OpenWrite(root + "\\sit.xml")) {
                    byte[] buffer = Encoding.UTF8.GetBytes(doc.Document);
                    stream.Write(buffer, 0, buffer.Length);
                }
            } catch { }

            if (doc != null)
                return doc;

            /*pornim procesul*/
            Process process = Process.Start(new ProcessStartInfo("java",
                " -jar AISituations.jar -f \"" + root + "\\info.xml\" " +
                " -t \"" + root + "\\main.txt\" " +
                " -o \"" + root + "\\sit.xml\""
                ) {
                    WorkingDirectory = ROOT + "\\sit"
                });

            process.WaitForExit();

            try {
                /*citim fisierul si il punem in bd*/
                using (Stream stream = File.OpenRead(root + "\\sit.xml")) {
                    StreamReader sr = new StreamReader(stream);

                    /*adaugam documentul in baza de date*/
                    doc = this._em.AddDocumentOutput(this._doc.DocumentId, "situations",
                        DocumentManager.StatusOK, sr.ReadToEnd());
                }
            } catch { };

            return doc;
        }

        private DocumentOutput _CombineTimeline(DocumentOutput summ, DocumentOutput sit, DocumentOutput time) {
            XmlDocument root = new XmlDocument();
            XmlDocument sitDoc = new XmlDocument(); sitDoc.LoadXml(sit.Document);
            XmlDocument timeDoc = new XmlDocument(); timeDoc.LoadXml(time.Document);
            XmlDocument summDoc = new XmlDocument(); summDoc.LoadXml(summ.Document);

            var situations = root.CreateElement("situations"); root.AppendChild(situations);
            /*infromatiile extrase anterior*/
            foreach (XmlElement elem in summDoc.GetElementsByTagName("extraction")) {
                situations.AppendChild(elem);                
            }

            foreach (XmlElement esit in sitDoc.GetElementsByTagName("situation")) {
                
            }

            MemoryStream ms = new MemoryStream();
            root.Save(ms);
            return this._em.AddDocumentOutput(this._doc.DocumentId, "summary", DocumentManager.StatusOK,
                Encoding.UTF8.GetString(ms.ToArray()));
        }
    }
}
