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

        private void FormClient_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.Delimiter = (byte)'\r';
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            client.WriteLineAndGetReply(txtMessage.Text, TimeSpan.FromSeconds(3));
            txtStatus.Text += "\n";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client.Connect(txtHost.Text, Convert.ToInt32(txtPort.Text));
                btnConnect.Enabled = false;
                btnSend.Enabled = true;
            }
            catch (Exception ex)
            {
                // Handle the exception here (e.g., display an error message).
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
