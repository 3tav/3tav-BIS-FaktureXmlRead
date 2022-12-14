using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace XmlExport
{
    class EGPZbirnikiTiskar : XmlExport
    {
        public override void InitParameters()
        {
            _sqlParameters.Add(new SqlParameter("@ARG_PAKET", SqlDbType.DateTime));
            _sqlParameters.Add(new SqlParameter("@ARG_ID_ZBIRNIK ", SqlDbType.Int));
        }
    }
}
