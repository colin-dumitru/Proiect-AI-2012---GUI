using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebServer.Models
{
    [Serializable]
    public class DocumentResponse : IXmlSerializable
    {
        /// <summary>
        /// Starea in care se afla un document.
        /// </summary>
        public enum Status { Finished, Parsing, Empty, NotFound}

        public String Content {get; set;}
        public Status State { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("State", State.ToString());
            if (this.Content != null)
            {
                writer.WriteRaw("<Content>");
                writer.WriteRaw(Content);
                writer.WriteRaw("</Content>");
            }
        }
    }
}