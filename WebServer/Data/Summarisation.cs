using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WebServer.Data {
    public class Summarisation {
        public string ProcessXML(XElement node) {
            StringBuilder sb = new StringBuilder();
            foreach (XElement child in node.Elements()) {
                if (String.CompareOrdinal(child.Name.ToString(), "characters") == 0) {
                    sb.Append(ProcessCharacters(child) + "\n");
                } else if (String.CompareOrdinal(child.Name.ToString(), "locations") == 0) {
                    sb.Append(ProcessLocations(child) + "\n");
                } else if (String.CompareOrdinal(child.Name.ToString(), "connections") == 0) {
                    foreach (XElement childC in child.Elements()) {
                        if (String.CompareOrdinal(childC.Name.ToString(), "relationships") == 0) {
                            //sb.Append(ProcessRelationships(childC) + "\n");
                        } else if (String.CompareOrdinal(childC.Name.ToString(), "actions") == 0) {
                            sb.Append(ProcessActions(childC) + "\n");
                        }
                    }
                }
            }
            return sb.ToString();
        }

        private String ProcessCharacters(XElement characters) {
            StringBuilder sb = new StringBuilder();
            sb.Append("The main character of this novel is ");
            List<string> secondaryCh = new List<string>();
            foreach (XElement x in characters.Elements()) {
                if (x.HasAttributes) {
                    foreach (XAttribute xAttr in x.Attributes()) {
                        if (String.CompareOrdinal(xAttr.Name.ToString(), "main") == 0) {
                            sb.Append(x.Value);
                            sb.Append(". Other secondary characters are ");
                        }
                        if (String.CompareOrdinal(xAttr.Name.ToString(), "secondary") == 0) {
                            secondaryCh.Add(x.Value);
                        }
                    }
                }
            }
            sb.Append(String.Join(", ", secondaryCh.Take(secondaryCh.Count - 1).ToArray()));
            sb.Append(" and ");
            sb.Append(secondaryCh.Last());
            sb.Append(". ");
            return sb.ToString();
        }

        private String ProcessLocations(XElement places) {
            StringBuilder sb = new StringBuilder();
            sb.Append("The action takes places in ");
            string place = places.Elements().First().Value;
            sb.Append(place);
            sb.Append(", ");
            sb.Append(GetWordnetOutput(place));
            sb.Append(", and also in ");
            string nextPlace = places.Elements().ElementAt(1).Value;
            sb.Append(nextPlace);
            sb.Append(", ");
            sb.Append(GetWordnetOutput(nextPlace));
            sb.Append(". ");
            return sb.ToString();
        }

        private String ProcessRelationships(XElement relations) {
            StringBuilder sb = new StringBuilder();
            foreach (XElement relation in relations.Elements()) {
                string e1 = null, e2 = null, link = null;
                foreach (XAttribute a in relation.Attributes()) {
                    if (String.CompareOrdinal(a.Name.ToString(), "entity1") == 0) {
                        e1 = a.Value;
                    }
                    if (String.CompareOrdinal(a.Name.ToString(), "entity2") == 0) {
                        e2 = a.Value;
                    }
                    if (String.CompareOrdinal(a.Name.ToString(), "link") == 0) {
                        link = a.Value;
                    }
                }
                sb.Append(e1);
                sb.Append(" is ");
                sb.Append(e2);
                sb.Append(" ");
                sb.Append(link);
                sb.Append("'s. ");
            }
            return sb.ToString();
        }

        private String ProcessActions(XElement actions) {
            StringBuilder sb = new StringBuilder();
            foreach (XElement action in actions.Elements()) {
                sb.Append(action.Value);
            }
            return sb.ToString();
        }

        public string GetWordnetOutput(string input) {
            Process pro = new Process {
                StartInfo = {
                    FileName = @"C:\lib\wordnet\bin\wn.exe",
                    WorkingDirectory = @"C:\lib\wordnet\bin\",
                    Arguments = "\"" + input + "\"" + " -over",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            pro.Start();

            string output = pro.StandardOutput.ReadToEnd();

            pro.WaitForExit();

            try {
                int startIndex = output.IndexOf("-- (", StringComparison.Ordinal);
                string substring = output.Substring(startIndex + 1);
                substring = substring.Replace(';', ',');
                int endIndex = substring.IndexOf(')');
                return substring.Substring(3, endIndex - 3);
            } catch {
                return "";
            }
        }

    }
}