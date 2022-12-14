using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace XmlExport
{
    class EGPObrestiTiskar : XmlExport
    {
        public override void InitParameters()
        {
            _sqlParameters.Add(new SqlParameter("@ARG_ID_OBRESTI", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_PP", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_DIM", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_DIM_INCLUDE", SqlDbType.Int));            
        }        
    }
}
