using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using XmlExport;

namespace XmlExportClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string server, database, user, pass, exportId, exportArgs, xmlPath;

            //MessageBox.Show(args.Length.ToString());
            //MessageBox.Show(string.Join("", args));

            if (args.Length == 7)
            {                
                try
                {                    
                    server = args[0];
                    database = args[1];
                    user = args[2];
                    pass = args[3];
                    exportId = args[4];
                    exportArgs = args[5];
                    xmlPath = args[6];                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Napaka pri branju argumentov!");
                    return;
                }          
            }
            else
            {
             //   MessageBox.Show("Napačno število argumentov za program!");
              //  return;                

                string podjetje = "ECE";

                switch (podjetje)
                { 
                    case "EGP":
                        server = "3tav-sql\\sql2012";
                        database = "DB_IS_EG_P_NOVA";
                        user = "";
                        pass = "";
                        exportId = "EGP_bis_kreirajRacunTiskarXML";
                        exportArgs = "2015-02-05 10:59:59.670;null";                        
                        xmlPath = "c:\\temp\\epps\\test_egp.xml";
                        break;
                    default:
                        server = "srvce96";
                        database = "DB_IS_ECE_DemoQ";
                        user = "3tav_admin";
                        pass = "3tav_Admin";
                        exportId = "ECE_bis_kreirajRacunTiskarXML_MPO";
                        exportArgs = "2015-04-01 17:57:32.377";
                        xmlPath = "C:\\temp\\MPO_150401175732.xml";
                        break;
                }                 
            }

            var fp = new Form1(args);
            if (fp.Init(server, database, user, pass, exportId, exportArgs, xmlPath) == false)
            {
                return;
            }

            //fp.lbl

            fp.ShowDialog();
         
        }
    }
}
