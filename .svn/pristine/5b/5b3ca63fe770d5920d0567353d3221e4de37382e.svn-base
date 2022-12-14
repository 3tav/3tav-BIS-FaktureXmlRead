using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlExport
{
    public abstract class XmlExport : IXmlExport
    {
        protected string _podjetje;
        protected string _storedProcedure;
        protected string _filePath;
        protected string _schema;
        protected bool _validateSchema;

        protected List<XmlZbirnikNapaka> _zbirnikiNapaka;

        public string Podjetje {get {return _podjetje;}}
        public string StoredProcedure {get {return _storedProcedure;}}
        public string FilePath {get {return _filePath;}}
        public string Schema {get {return _schema;}}
        public bool ValidateSchema { get { return _validateSchema;}}

        protected string _parameterValues;
        protected List<SqlParameter> _sqlParameters;
        protected const char _parmDelimiter = ';';
        protected const string _nullString = "null";
        protected string _connString;

        public XmlExport()
        {
            _sqlParameters = new List<SqlParameter>();
            _zbirnikiNapaka = new List<XmlZbirnikNapaka>();
        }

        public virtual void Init(string connString, string exportId, string parameterValues, string filePath, bool validateSchema)
        {    
            // parametri izvoza
            _connString = connString;
            _parameterValues = parameterValues;
            _podjetje = exportId.Substring(0, exportId.IndexOf("_"));
            _storedProcedure = exportId.Substring(exportId.IndexOf("_") +1);
            _schema = string.Format("{0}.xsd", exportId);
            _filePath = filePath;
            _validateSchema = validateSchema;

            // stored procedure parametri
            InitParameters();
        }

        public abstract void InitParameters();

        public virtual void CreateXml()
        {
            string xml = string.Empty;

                      
            GetParameterValues(_parameterValues);
            using (var conn = new SqlConnection(_connString))
            {
            
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // generiranje xml
                    cmd.CommandText = StoredProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddRange(_sqlParameters.ToArray());
                    using (var rdr = cmd.ExecuteXmlReader())
                    {
                        if (rdr.Read())
                        {
                            xml = rdr.ReadOuterXml();
                        }
                    }
                }
            
                // zapis v datoteko
                WriteToFile(FilePath, xml);

                // xsd validacija                
                Validate(Schema, FilePath);
            }
        }
      
        protected void GetParameterValues(string values)
        {
            if (values == null)
                throw new Exception("Null parametri");

            var parameters = values.Split(_parmDelimiter);

            if (parameters.Length != _sqlParameters.Count)
                throw new Exception(string.Format("Število parametrov se ne ujema z definicijo ({0} namesto {1})!", parameters.Length, _sqlParameters.Count));

            for (var i = 0; i < _sqlParameters.Count; i++)
            {
                _sqlParameters[i].Value = GetValue(parameters[i], _sqlParameters[i].SqlDbType);
            }
        }

        protected object GetValue(string strValue, SqlDbType type)
        {
            object value = null;
            switch (type)
            { 
                case SqlDbType.Int:
                    int? intVal = GetInt(strValue);
                    if (intVal == null)
                    {
                        value = DBNull.Value;
                    }
                    else
                    {
                        value = intVal.Value;
                    }                                        
                    break;
                case SqlDbType.DateTime:
                    DateTime? dateTimeVal = GetDateTime(strValue);
                    if (dateTimeVal == null)
                    {
                        value = DBNull.Value;
                    }
                    else
                    {
                        value = dateTimeVal.Value;
                    }
                    break;
                default:
                    string s = GetString(strValue);
                    if (s == null)
                    {
                        value = DBNull.Value;
                    }
                    else
                    {
                        value = s;
                    }
                    break;
            }      
            return value;
        }
        protected string GetString(string stringVal)
        {

            string retVal = stringVal;

            if (retVal == _nullString)
                retVal = null;

            return retVal;
        }
        protected DateTime? GetDateTime(string stringVal)
        {
            DateTime? retVal = null;
            DateTime parseVal;

            if (DateTime.TryParse(stringVal, out parseVal))
                retVal = parseVal;

            return retVal;
        }
        protected int? GetInt(string stringVal)
        {
            int? retVal = null;
            int parseVal;

            if (int.TryParse(stringVal, out parseVal))
                retVal = parseVal;

            return retVal;
        }

        protected void WriteToFile(string filePath, string xml)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                XmlDocument doc;
                doc = new XmlDocument();

                doc.LoadXml(xml);

                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                settings.IndentChars = "  ";
                settings.NewLineChars = "\r\n";
                settings.NewLineHandling = NewLineHandling.Replace;
                using (XmlWriter writer = XmlWriter.Create(filePath, settings))
                {
                    doc.Save(writer);
                }
            }
            catch (Exception ex)
            {
                throw new FileSystemException(ex.Message);
            }
            
        }     



        public void Validate(string schema, string fileName)
        {
            //if (ValidateSchema == false)
            //    return;
            
            string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string xsdFolder = Path.Combine(currentDirectory, "xsd");
            string schemaFile = Path.Combine(xsdFolder, schema);

            if (File.Exists(schemaFile) == false)
            {
                throw new Exception(string.Format("Shema {0} ne obstaja!", schemaFile));
            }
            
            //throw new XsdValidatorException("Napaka pri validaciji XML!");

            var validator = new XsdValidator();
            var xmlErrors = string.Empty;
            validator.AddSchema(schemaFile);
            if (validator.IsValid(fileName) == false)
            {
                for (int j = 0; j < validator.Errors.Count; j++)
                {
                    var xmlError = String.Format("Napaka: {0} \t Line: {1} \t Position: {2}\n", validator.Errors[j], validator.ErrLine[j], validator.ErrRows[j]);
                    var zbirniki = xmlError.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var z in zbirniki)
                    { 
                        int idZbirnika = 0;
                        if (int.TryParse(z, out idZbirnika))
                        {
                            if (idZbirnika > 0)
                                _zbirnikiNapaka.Add(new XmlZbirnikNapaka(idZbirnika, xmlError));
                        }
                    }

                    // najdi problematične xml                    
                    xmlErrors += xmlError;
                }
                
                throw new XsdValidatorException(xmlErrors);
            }            
        }

        protected int GetValidationFailedElementId(string xmlFile, int lineNumber)
        {

            XDocument xml = XDocument.Load(xmlFile, LoadOptions.SetLineInfo);

            var line = from x in xml.Elements("DokumentTiskar")
                       let lineInfo = (IXmlLineInfo)x
                       where (lineInfo.LineNumber == lineNumber)
                       select x;

            foreach (var item in line)
            {
                Console.WriteLine(item);
            }

            return 0;
        }

    }

    public class XmlZbirnikNapaka
    { 
        public int IdZbirnika;
        public string Napaka;
        public XmlZbirnikNapaka(int idZbirnika, string napaka)
        { 
            IdZbirnika = idZbirnika;
            Napaka = napaka;
        }
    }
}
