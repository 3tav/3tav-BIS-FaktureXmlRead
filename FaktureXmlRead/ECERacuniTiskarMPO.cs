using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace XmlExport
{
    class ECERacuniTiskarMPO : XmlExport
    {
        private const string Tip = "MPO";
        private const string StatusOK = "D";
        private const string StatusError = "N";
        private const string StatusInProgress = "V";
        public override void InitParameters()
        {
            _storedProcedure = "bis_KreirajRacunTiskarXML";
            _sqlParameters.Add(new SqlParameter("@ARG_PAKET", SqlDbType.DateTime));         
        }

        public override void CreateXml()
        {
            string status = StatusInProgress;
            string message = string.Empty;
            var parms = _parameterValues.Split(_parmDelimiter);            
            var paket = GetDateTime(parms[0]);
            var napaka = false;

            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();                
                // nastavi status v izpisu
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"update bis_izbor_zbirniki_gl 
                                            set status = @status,
                                                    napaka = @napaka,
                                                    datoteka = @datoteka
                                            where paket = @paket";
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@napaka", string.Empty);
                    cmd.Parameters.AddWithValue("@datoteka", string.Empty);
                    cmd.Parameters.AddWithValue("@paket", paket);
                    cmd.ExecuteNonQuery();
                }
                

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"update bis_izbor_zbirniki 
                                            set status = @status,
                                                    napaka = @napaka,
                                                    datoteka = @datoteka
                                            where paket = @paket and 
                                                    tip = @tip";
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@napaka", string.Empty);
                    cmd.Parameters.AddWithValue("@datoteka", string.Empty);
                    cmd.Parameters.AddWithValue("@paket", paket);
                    cmd.Parameters.AddWithValue("@tip", Tip);
                    cmd.ExecuteNonQuery();
                }
                
            }

            try
            {
                base.CreateXml();
                status = StatusOK;
            }
            catch (Exception ex)
            {
                status = StatusError;
                message = ex.Message;
                napaka = true;
            }
            finally
            {

                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();


                    if (napaka == true)
                    {
                        // napake na posameznih zbirnikih             
                        if (_zbirnikiNapaka.Count > 0)
                        {
                            foreach (var z in _zbirnikiNapaka)
                            {
                                using (var cmd = conn.CreateCommand())
                                {
                                    cmd.CommandText = @"update bis_izbor_zbirniki 
                                                set status = @status,
                                                        napaka = @napaka
                                                where paket = @paket and
                                                        IdZbirnika = @IdZbirnika and
                                                        tip = @tip";
                                    cmd.Parameters.AddWithValue("@status", status);
                                    cmd.Parameters.AddWithValue("@napaka", z.Napaka);
                                    cmd.Parameters.AddWithValue("@paket", paket);
                                    cmd.Parameters.AddWithValue("@IdZbirnika", z.IdZbirnika);
                                    cmd.Parameters.AddWithValue("@tip", Tip);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            // globalna napaka na celotnem paketu
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"update bis_izbor_zbirniki_gl 
                                            set status = @status,
                                                    napaka = @napaka
                                            where paket = @paket";
                                cmd.Parameters.AddWithValue("@status", status);
                                cmd.Parameters.AddWithValue("@napaka", message);
                                cmd.Parameters.AddWithValue("@paket", paket);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        // OK
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"update bis_izbor_zbirniki_gl 
                                            set status = @status,
                                                    napaka = @napaka,
                                                    datoteka = @datoteka
                                            where paket = @paket";
                            cmd.Parameters.AddWithValue("@status", status);
                            cmd.Parameters.AddWithValue("@napaka", string.Empty);
                            cmd.Parameters.AddWithValue("@paket", paket);
                            cmd.Parameters.AddWithValue("@datoteka", base._filePath);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"update bis_izbor_zbirniki 
                                            set status = @status,
                                                    napaka = @napaka,
                                                    datoteka = @datoteka
                                            where paket = @paket and
                                                  tip = @tip";
                            cmd.Parameters.AddWithValue("@status", status);
                            cmd.Parameters.AddWithValue("@napaka", string.Empty);
                            cmd.Parameters.AddWithValue("@paket", paket);
                            cmd.Parameters.AddWithValue("@datoteka", base._filePath);
                            cmd.Parameters.AddWithValue("@tip", Tip);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }          
    }
}
