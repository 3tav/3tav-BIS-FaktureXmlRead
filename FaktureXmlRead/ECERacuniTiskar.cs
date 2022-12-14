using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace XmlExport
{
    class ECERacuniTiskar : XmlExport
    {
        public override void InitParameters()
        {            
            _sqlParameters.Add(new SqlParameter("@PAKET", SqlDbType.Int));
            _sqlParameters.Add(new SqlParameter("@STEVILKARACUNA ", SqlDbType.Int));            
        }

        public override void CreateXml()
        {
            
            string ret = string.Empty;
            string xml = string.Empty;

            

            //GetParameterValues(_parameterValues);

            var parms = _parameterValues.Split(_parmDelimiter);            

            var paket = GetDateTime(parms[0]);

            //try
            //{
            var filename = Path.GetFileName(FilePath);
            var filenameMPO = string.Format("MPO_{0}", filename);
            var filenameVPO = string.Format("VPO_{0}", filename);

            var filePathMPO = FilePath.Replace(filename, filenameMPO);
            var filePathVPO = FilePath.Replace(filename, filenameVPO);

            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();

                try
                {
                    // prva procedura za MPO
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "bis_KreirajRacunTiskarXML";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.AddWithValue("@ARG_IdZbirnika", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ARG_STEVILKA_FAKTURE", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ARG_PAKET", paket);

                        using (var rdr = cmd.ExecuteXmlReader())
                        {
                            if (rdr.Read())
                            {
                                xml = rdr.ReadOuterXml();
                            }
                        }
                    }

                    WriteToFile(filePathMPO, xml);

                    Validate("ECE_bis_KreirajRacunTiskarXML_MPO.xsd", filePathMPO);


                }                
                catch (Exception ex)
                {                    
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"update bis_izbor_zbirniki 
                                            set status = @status,
                                                 napaka = @napaka
                                            where paket = @paket";
                        cmd.Parameters.AddWithValue("@status", "N");
                        cmd.Parameters.AddWithValue("@napaka", ex.Message);
                        cmd.Parameters.AddWithValue("@paket", paket);
                        cmd.ExecuteNonQuery();                        
                    }

                    throw new Exception(string.Format("{0} [MPO]", ex.Message));
                }
                
                // druga procedura za VPO
                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        xml = string.Empty;
                        cmd.CommandText = "bis_KreirajRacunTiskarXML_VPO";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.AddWithValue("@ARG_IdZbirnika_VPO", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ARG_PAKET_VPO", paket);

                        using (var rdr = cmd.ExecuteXmlReader())
                        {
                            if (rdr.Read())
                            {
                                xml = rdr.ReadOuterXml();
                            }
                        }

                    }
                    WriteToFile(filePathVPO, xml);

                    Validate("ECE_bis_KreirajRacunTiskarXML_VPO.xsd", filePathVPO);
                }
                catch (Exception ex)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"update bis_izbor_zbirniki_vpo 
                                            set status = @status,
                                                 napaka = @napaka
                                            where paket = @paket";
                        cmd.Parameters.AddWithValue("@status", "N");
                        cmd.Parameters.AddWithValue("@napaka", ex.Message);
                        cmd.Parameters.AddWithValue("@paket", paket);
                        cmd.ExecuteNonQuery();
                    }
                    throw new Exception(string.Format("{0} [VPO]", ex.Message));
                }
            }
            /*
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            */
            //return ret;
        }


    }
}
