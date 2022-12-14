using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.ComponentModel;

using XmlExport;

namespace XmlExportClient
{
    public partial class Form1 : Form
    {

        public XmlExportService _service;
        public int _tickCount;
        public bool _debug = false;

        public Form1(string[] args)
        {
            InitializeComponent();
            progressBar1.Visible = false;            
        }
      
        public bool Init(string server, string database, string user, string pass, string storedProc, string storedProcArgs, string xmlPath)
        {
            try
            {
                DisplayParms(server, database, user, pass, storedProc, storedProcArgs, xmlPath);

                _service = new XmlExportService();
                _service.Init(server, database, user, pass, storedProc, storedProcArgs, xmlPath);
                                
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Napaka pri inicializaciji XML servisa: {0}", ex.Message), "Napaka pri inicializaciji XML servisa!");
                Close();
                return false;
            }

            lblStatus.Text = string.Format("Izpis: {0}", storedProc);
            lblFileName.Text = string.Format("Datoteka: {0}", xmlPath.Replace("\\\\", "\\"));

            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {         
            backgroundWorker1.RunWorkerAsync();
        }
      
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate() { this.progressBar1.Visible = true; timer1.Start(); btnCancel.Enabled = false; btnRun.Enabled = false; });
                _service.CreateXml();
                this.Invoke((MethodInvoker)delegate() { this.progressBar1.Visible = false; timer1.Stop(); lblStatus.Text = "Končano."; btnCancel.Enabled = true; btnRun.Enabled = true; });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate() { this.progressBar1.Visible = false; timer1.Stop(); btnCancel.Enabled = true; btnRun.Enabled = true; });
                MessageBox.Show(ex.Message, "Napaka pri kreiranju XML");
                if (!_debug)
                    this.Invoke((MethodInvoker)delegate() { this.Close(); });
                return;
            }
            this.Invoke((MethodInvoker)delegate() { this.progressBar1.Visible = false; timer1.Stop(); this.Close(); });
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // MessageBox.Show(e.Error.Message);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _tickCount++;
            labelTime.Text = string.Format("{0:D2}:{1:D2}", TimeSpan.FromSeconds(_tickCount).Minutes, TimeSpan.FromSeconds(_tickCount).Seconds);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // reevaluate, test run
            if (!Init(inputServer.Text, inputDatabase.Text, inputUser.Text, inputPassword.Text, inputProcedure.Text, inputArguments.Text, inputPath.Text))
            {
                return;
            }

            backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DisplayParms(string server, string database, string user, string pass, string storedProc, string storedProcArgs, string xmlPath)
        {
            inputServer.Text = server;
            inputDatabase.Text = database;
            inputUser.Text = user;
            inputPassword.Text = pass;
            inputProcedure.Text = storedProc;
            inputArguments.Text = storedProcArgs;
            inputPath.Text = xmlPath;        
        }

    }
}
