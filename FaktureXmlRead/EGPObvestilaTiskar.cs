using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace XmlExport
{
    class EGPObvestilaTiskar : XmlExport
    {                
        public override void InitParameters()
        {
            _sqlParameters.Add(new SqlParameter("@ARG_OBVESTILO_PAKET", SqlDbType.DateTime));
            _sqlParameters.Add(new SqlParameter("@ARG_PP", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_MM", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_NIVO", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_UPN_VISIBLE", SqlDbType.Int));            
        }        
    }
}
