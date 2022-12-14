using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace XmlExport
{
    class EGPOpominiTiskar : XmlExport
    {
        public override void InitParameters()
        {
            _sqlParameters.Add(new SqlParameter("@ARG_OPOMIN_GLAVA_ID", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_VRSTA", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_NIVO", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_PP", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_DIM", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_UPN_VISIBLE", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@ARG_DIM_INCLUDE", SqlDbType.Int));
        }
    }
}
