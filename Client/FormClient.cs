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

namespace Client
{
    public partial class FormClient : Form
    {
        public FormClient()
        {
            InitializeComponent();
            txtHost.Text = "127.0.0.1";
            txtPort.Text = "8080";
        } 

        SimpleTcpClient client;

        private void btnSend_Click(object sender, EventArgs e)
        {
            client.WriteLineAndGetReply(txtMessage.Text, TimeSpan.FromSeconds(3));
            txtStatus.Text += "\n";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            client.Connect(txtHost.Text, Convert.ToInt32(txtPort.Text));
            btnConnect.Enabled = false;
            btnSend.Enabled = true;     
        }

        private void FormClient_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            //client.Delimiter = 0x13; //enter
            client.Delimiter = (byte)'\r';

            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker) delegate ()
            {
                txtStatus.Text += e.MessageString;
                txtStatus.AppendText(Environment.NewLine);
            });
        }
    }
    
}
