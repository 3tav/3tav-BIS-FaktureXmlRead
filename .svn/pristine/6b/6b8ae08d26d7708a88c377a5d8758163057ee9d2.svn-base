using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace XmlExport
{
    class XsdValidator
    {
        public List<XmlSchema> Schemas { get; set; }
        public List<String> Errors { get; set; }
        public List<String> Warnings { get; set; }

        public List<int> ErrLine { get; set; }
        public List<int> ErrRows { get; set; }

        private string _lastId = string.Empty;            

        public XsdValidator()
        {
            Schemas = new List<XmlSchema>();
        }

        /// <summary>
        /// Add a schema to be used during the validation of the XML document
        /// </summary>
        /// <param name="schemaFileLocation">The file path for the XSD schema file to be added for validation</param>
        /// <returns>True if the schema file was successfully loaded, else false (if false, view Errors/Warnings for reason why)</returns>
        public bool AddSchema(string schemaFileLocation)
        {
            if (String.IsNullOrEmpty(schemaFileLocation)) return false;
            if (!File.Exists(schemaFileLocation)) return false;

            // Reset the Error/Warning collections
            Errors = new List<string>();
            Warnings = new List<string>();

            ErrLine = new List<int>();
            ErrRows = new List<int>();

            XmlSchema schema;

            using (var fs = File.OpenRead(schemaFileLocation))
            {
                schema = XmlSchema.Read(fs, ValidationEventHandler);
            }

            var isValid = !Errors.Any() && !Warnings.Any();

            if (isValid)
            {
                Schemas.Add(schema);
            }

            return isValid;
        }

        /// <summary>
        /// Perform the XSD validation against the specified XML document
        /// </summary>
        /// <param name="xmlLocation">The full file path of the file to be validated</param>
        /// <returns>True if the XML file conforms to the schemas, else false</returns>
        public bool IsValid(string xmlLocation)
        {
            if (!File.Exists(xmlLocation))
            {
                throw new FileNotFoundException("The specified XML file does not exist", xmlLocation);
            }

            using (var xmlStream = File.OpenRead(xmlLocation))
            {
                return IsValid(xmlStream);
            }
        }

        /// <summary>
        /// Perform the XSD validation against the supplied XML stream
        /// </summary>
        /// <param name="xmlStream">The XML stream to be validated</param>
        /// <returns>True is the XML stream conforms to the schemas, else false</returns>
        private bool IsValid(Stream xmlStream)
        {
            // Reset the Error/Warning collections
            Errors = new List<string>();
            Warnings = new List<string>();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };
            settings.ValidationEventHandler += ValidationEventHandler;

            foreach (var xmlSchema in Schemas)
            {
                settings.Schemas.Add(xmlSchema);
            }

            var xmlFile = XmlReader.Create(xmlStream, settings);
            
            bool stRacuna = false;
            int idZbirnika = 0;
            try
            {
                while (xmlFile.Read()) {

                    if (stRacuna)
                    {
                        if (int.TryParse(xmlFile.Value, out idZbirnika))
                            _lastId = xmlFile.Value;

                        stRacuna = false;
                    }

                    stRacuna = false;
                    if (xmlFile.Name == "StevilkaRacuna")
                    {
                        stRacuna = true;                        
                    }
                    

                }
            }
            catch (XmlException xex)
            {                
                Errors.Add(string.Format("[{0}] {1}", _lastId, xex.Message));
            }

            return !Errors.Any() && !Warnings.Any();
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Errors.Add(string.Format("[{0}] {1}",_lastId, e.Message));                    
                    ErrLine.Add(e.Exception.LineNumber);
                    ErrRows.Add(e.Exception.LinePosition);
                    break;
                case XmlSeverityType.Warning:
                    Warnings.Add(e.Message);
                    break;
            }
        }
    }
}
