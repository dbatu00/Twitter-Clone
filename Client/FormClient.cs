using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public partial class FormClient : Form
    {
        public FormClient()
        {
            InitializeComponent();
            //hardcoded for convenience
            txtHost.Text = "127.0.0.1";
            txtPort.Text = "8080";
            txtUsername.Text = "user1";
            txtPassword.Text = "password1";
        } 

        private TcpClient client;
        private NetworkStream stream;

        private int bytesRead;
        private byte[] receiveBuffer = new byte[1024];

        private void FormClient_Load(object sender, EventArgs e)
        {
            client = new TcpClient();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            stream.Write(Encoding.UTF8.GetBytes("MSG" + " " + txtMessage.Text), 0, txtMessage.Text.Length + 4);
            txtStatus.Text += "\n";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client.Connect(txtHost.Text, Convert.ToInt32(txtPort.Text));
                stream = client.GetStream();


                Thread receiveThread = new Thread(() =>
                {
                    while (true)
                    {
                        bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                        if (bytesRead > 0)
                        {
                            string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                            // Handle the received data (e.g., display it or process it)
                            Client_DataReceived(receivedData);
                        }
                    }
                });
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                // Handle the exception here (e.g., display an error message).
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Client_DataReceived(string message)
        {

            if (message == "Connection succesfull.")
            {
                txtStatus.Invoke((MethodInvoker)delegate
                {
                    btnConnect.Enabled = false;
                    btnLogin.Enabled = true;
                });
            }
            else if (message == "Authentication successful.")
            {
                txtStatus.Invoke((MethodInvoker)delegate
                {
                    btnLogin.Enabled = false;
                    btnSend.Enabled = true;
                });
            }

            if (txtStatus.InvokeRequired)
            {
                // If called from a different thread, invoke on the UI thread
                txtStatus.Invoke((MethodInvoker)delegate
                {
                    txtStatus.Text += "Server says: " + message;
                    txtStatus.AppendText(Environment.NewLine);
                });
            }
            else
            {
                // If already on the UI thread, update directly
                txtStatus.Text += "Server says: " + message;
                txtStatus.AppendText(Environment.NewLine);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string message = "LOGIN" + " " + txtUsername.Text + " " + txtPassword.Text; 
            stream.Write(Encoding.UTF8.GetBytes(message), 0, message.Length);
        }
    }

}
