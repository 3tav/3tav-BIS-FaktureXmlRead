using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Xml;
using System.Linq;

namespace XmlExport
{    
    public class XmlExportService
    {
        private string _connString;                                
        private string _storedProc;
        private string _storedProcArgs;
        private string _xmlPath;
        
        public void Init(string server, string database, string user, string pass, string storedProc, string storedProcArgs, string xmlPath)
        {            
            _connString = GetConnectionString(server, database, user, pass);            
            _storedProc = storedProc;
            _storedProcArgs = storedProcArgs;
            _xmlPath = xmlPath;
        }
        
        private string GetConnectionString(string server, string database, string user, string pass)
        {
            
            //user = "3tav_admin";
            //pass = "3tav_Admin";
            //string trustedConnection = "No";
            
            
            string trustedConnection = "No";
            if (string.IsNullOrEmpty(user))
            {
                trustedConnection = "Yes";
            }
            
            return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Trusted_Connection={4};Connection Timeout=600;", server, database,user, pass, trustedConnection);
        }

        //  main method wrapper zaradi async klica
        public void CreateXml()
        {
            CreateXml(_storedProc, _storedProcArgs, _xmlPath);
        }

        // main method
        public void CreateXml(string exportId, string exportArgs, string filePath)
        {            
            var export = GetExport(exportId);
            export.Init(_connString, exportId, exportArgs, filePath, true);            
            export.CreateXml();           
            //export.Validate(@"C:\temp\egp\EGP_bis_KreirajRacunTiskarXML.XSD", @"C:\temp\egp\02_Obracun_akontacija_PO_150326075243.xml");
            

        }

        // export mapper
        private IXmlExport GetExport(string exportId)
        { 
            switch (exportId)
            {
                case "ECE_bis_kreirajRacunTiskarXML":
                    return new ECERacuniTiskar();
                case "ECE_bis_kreirajRacunTiskarXML_MPO":
                    return new ECERacuniTiskarMPO();
                case "ECE_bis_kreirajRacunTiskarXML_VPO":
                    return new ECERacuniTiskarVPO();
                case "EGP_bis_kreirajRacunTiskarXML":
                    return new EGPRacuniTiskar();                
                case "EGP_fin_kreirajObvestilaTiskarXML":
                case "ECE_fin_kreirajObvestilaTiskarXML":
                    return new EGPObvestilaTiskar();
                case "EGP_fin_kreirajOpominTiskarXML":
                case "ECE_fin_kreirajOpominTiskarXML":
                    return new EGPOpominiTiskar();
                case "EGP_bis_kreirajZbirnikTiskarXML":
                    return new EGPZbirnikiTiskar();
                case "EGP_fin_kreirajObrestiTiskarXML":
                case "ECE_fin_kreirajObrestiTiskarXML":
                    return new EGPObrestiTiskar();
                case "REK_IzvozXML":
                    return new REKIzvozXML();
                default: 
                    throw new Exception(string.Format("Izvoz {0} ni implementiran!", exportId));
            }            
        }        
    }
}

