using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormServer : Form
    {
        private TcpListener server;
        private List<TcpClient> clients = new List<TcpClient>();
        List<ClientInfo> loggedInClients = new List<ClientInfo>();

        Dictionary<string, string> userCredentials = new Dictionary<string, string>
        {
            { "user1", "password1" },
            { "user2", "password2" },
        };
        public class ClientInfo
        {
            public TcpClient TcpClient { get; set; }
            public string Username { get; set; }
        }

        bool listening = false;

        public FormServer()
        {
            InitializeComponent();
        }

        private void FormServer_Load(object sender, EventArgs e)
        {
            // Hardcoded for convenience
            txtHost.Text = "127.0.0.1";
            txtPort.Text = "8080";
        }

        private void Server_DataReceived(TcpClient client, string message)
        {
            string[] msgParts = message.Split(' ');
            string command = msgParts[0].Trim();

            switch (command)
            {
                case "LOGIN":
                    if (msgParts.Length == 3)
                    {
                        string username = msgParts[1].Trim();
                        string password = msgParts[2].Trim();
                        
                        if (ValidateUser(username, password))
                        {
                            ClientInfo clientInfo = new ClientInfo
                            {
                                TcpClient = client,
                                Username = username
                            };
                            loggedInClients.Add(clientInfo);

                            // Handle authentication response (you can use a separate method)
                            SendResponseToClient("Authentication successful.", client);
                        }
                        else
                        {
                            // Handle authentication failure (you can use a separate method)
                            SendResponseToClient("Authentication failed.", client);
                        }
                    }
                    else
                    {
                        // Handle invalid request format (you can use a separate method)
                        SendResponseToClient("Invalid LOGIN request format.", client);
                        UpdateStatus($"Invalid LOGIN request by {((TcpClient)client).Client.RemoteEndPoint}." +
                                     $"{Environment.NewLine}");
                    }
                    break;

                case "MSG":
                    SendResponseToClient($"You have said: {message}", client);
                    //UpdateStatus("");
                    //ClientInfo foundClient = clients.Find(client => client.Client.ToString == (sender as TcpClient).ToString);
                    //loggedInClients.Find
                    break;

                default:
                    // Handle unknown command (you can use a separate method)
                    SendResponseToClient("Unknown command.", client);
                    UpdateStatus($"Received UNKNOWN command by {((TcpClient)client).Client.RemoteEndPoint}." +
                                 $"{Environment.NewLine}");
                    break;
            }
        }

        private bool ValidateUser(string username, string password)
        {
            if (userCredentials.TryGetValue(username, out string storedPassword)) // does the username exist in user list?
            {
                if (storedPassword != password) //incorrect password
                {
                    UpdateStatus($"User: {username} entered wrong password." + Environment.NewLine);
                    return false;
                }
                else //correct password
                {
                    UpdateStatus($"User: {username} connected." + Environment.NewLine);
                    return true;
                }            
            }
            UpdateStatus($"Unknown actor with username: {username} had a failed connection attempt.");
            return false;
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            if (!listening)
            {
                StartServer();
                listening = true;
                btnStop.Enabled = true;
                btnListen.Enabled = false;
            }
        }

        private void StartServer()
        {
            string host = txtHost.Text;
            int port = int.Parse(txtPort.Text);

            server = new TcpListener(IPAddress.Parse(host), port);
            server.Start();

            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.Start();
            txtStatus.Text += "Server started...";
            txtStatus.Text += Environment.NewLine;
        }

        private void AcceptClients()
        {
            while (listening)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    clients.Add(client);
                    SendResponseToClient("Connection succesfull.", client);

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    if (listening)
                    {
                        UpdateStatus("An error occurred while listening: " + ex.Message + Environment.NewLine);
                    }
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                UpdateStatus($"Client:{client.Client.RemoteEndPoint} connected.{Environment.NewLine}");

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Process the received message
                    Server_DataReceived(client, message);

                    Array.Clear(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("An error occurred with the client: " + client.Client.RemoteEndPoint
                                                                    + ex.Message + Environment.NewLine);
            }
            finally
            {
                UpdateStatus("Client removed: " + client.Client.RemoteEndPoint + Environment.NewLine);
                clients.Remove(client);
                client.Close();
            }
        }

        private void SendResponseToClient(string response, TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(response);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                UpdateStatus("An error occurred while sending a response: " + ex.Message + Environment.NewLine);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (listening)
            {
                listening = false;
                server.Stop();
                txtStatus.Text += "Server stopped.";
                txtStatus.Text += Environment.NewLine;
                btnListen.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void UpdateStatus(string message)
        {
            txtStatus.Invoke((MethodInvoker)delegate
            {
                txtStatus.Text += message;
            });
        }
    }
}
