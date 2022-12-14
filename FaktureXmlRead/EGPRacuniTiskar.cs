using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace XmlExport
{
    class EGPRacuniTiskar : XmlExport
    {
        public override void InitParameters()
        {            
            _sqlParameters.Add(new SqlParameter("@PAKET", SqlDbType.DateTime));
            _sqlParameters.Add(new SqlParameter("@STEVILKARACUNA ", SqlDbType.Int));
        }
    }
}
