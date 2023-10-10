using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTCP;

namespace WindowsFormsApp1
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent();
            txtHost.Text = "127.0.0.1";
            txtPort.Text = "8080";
        }

        SimpleTcpServer server;

        private void btnListen_Click(object sender, EventArgs e)
        {
            txtStatus.Text += "Server starting...\n";
            System.Net.IPAddress ip;
            System.Net.IPAddress.TryParse(txtHost.Text, out ip);
            server.Start(ip, Convert.ToInt32(txtPort.Text));
        }

        private void FormServer_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            //server.Delimiter = 0x13; //enter
            //server.Delimiter = Encoding.UTF8.GetBytes("\r"); // Set the delimiter to carriage return
            server.Delimiter = (byte)'\r';
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Text += e.MessageString;
                txtStatus.AppendText(Environment.NewLine);
                e.ReplyLine(string.Format("You said :{0}", e.MessageString));
            });
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
                server.Stop();
        }

        private void txtHost_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
