using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

namespace XmlExport
{
    class REKIzvozXML : XmlExport
    {
        private const string _formatXML = "xml";
        private const string _formatHTML = "html";

        public override void InitParameters()
        {
            _storedProcedure = "od_rek_o_izvozXML";                        
        }

        public override void CreateXml()
        {
            var parms = _parameterValues.Split(_parmDelimiter);
            var izracun = GetInt(parms[0]);
            var formats = GetString(parms[1]).Split(',');
            var xml = string.Empty;
            using (var conn = new SqlConnection(_connString))
            {

                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // generiranje xml
                    cmd.CommandText = StoredProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@Izracun", izracun);
                    using (var rdr = cmd.ExecuteXmlReader())
                    {
                        if (rdr.Read())
                        {
                            xml = rdr.ReadOuterXml();
                        }
                    }
                }

             
            }

            // zapis v datoteko                
            foreach (var format in formats)
            {
                if (format == _formatXML)
                {
                    WriteToFile(FilePath, xml);
                }

                if (format == _formatHTML)
                {
                    var xslt = File.ReadAllText("C:\\temp\\rek\\REK_O_1.1-display-sl.xslt");
                    var htmlObrazec = TransformXMLToHTML(xml, xslt);

                    var filePath = Path.GetDirectoryName(FilePath);
                    var fileNameHTML = Path.GetFileName(FilePath).Replace(".xml", ".html");
                    var filePathHTML = string.Format("{0}\\{1}", filePath, fileNameHTML);

                    File.WriteAllText(filePathHTML, htmlObrazec);
                }
            }

            
        }
        public string TransformXMLToHTML(string inputXml, string xsltString)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(new StringReader(xsltString)))
            {
                transform.Load(reader);
            }
            StringWriter results = new StringWriter();
            using (XmlReader reader = XmlReader.Create(new StringReader(inputXml)))
            {
                transform.Transform(reader, null, results);
            }
            return results.ToString();
        }
        


    }
}
