using System;
namespace XmlExport
{
    interface IXmlExport
    {
        void CreateXml();
        void Init(string connString, string storedProcedure, string parameterValues, string filePath, bool validateSchema);
        void InitParameters();
        void Validate(string schema, string fileName);
    }
}
