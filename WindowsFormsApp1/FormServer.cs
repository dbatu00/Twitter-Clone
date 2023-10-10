using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using SimpleTCP;


/* 

class Program
{
   

    static void Main(string[] args)
    {
        try
        {
            server = new TcpListener(IPAddress.Any, 8080);
            server.Start();

            Console.WriteLine("Server started. Listening for clients...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                clients.Add(client);

                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        finally
        {
            server.Stop();
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            Console.WriteLine("Client connected.");

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + message);

                // Broadcast the message to all connected clients
                BroadcastMessage(message);

                // Clear the buffer for the next read
                Array.Clear(buffer, 0, buffer.Length);
            }

            Console.WriteLine("Client disconnected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred with a client: " + ex.Message);
        }
        finally
        {
            clients.Remove(client);
            client.Close();
        }
    }

    static void BroadcastMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (TcpClient client in clients)
        {
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }
    }
}
*/
namespace WindowsFormsApp1
{
    public partial class FormServer : Form
    {

        static SimpleTcpServer server;
        static List<SimpleTcpClient> clients = new List<SimpleTcpClient>();

        bool listening = false;

        public FormServer()
        {
            InitializeComponent();
        }    

        private void FormServer_Load(object sender, EventArgs e)
        {
            //hardcoded for convenience
            txtHost.Text = "127.0.0.1";
            txtPort.Text = "8080";

            server = new SimpleTcpServer();
            server.Delimiter = (byte)'\r';
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived; //subscribe event to function
        }
        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate () //ensuring UI update takes place on the UI thread
            {
                txtStatus.Text += e.MessageString;
                txtStatus.AppendText(Environment.NewLine);
                e.ReplyLine(string.Format("You said :{0}", e.MessageString));
            });
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            txtStatus.Text += "Server starting...\n";
            
            //start server
            System.Net.IPAddress ip;
            System.Net.IPAddress.TryParse(txtHost.Text, out ip);
            server.Start(ip, Convert.ToInt32(txtPort.Text));
            
            //global parameters
            listening = true;
            btnStop.Enabled = true;
            
            //start client accepting thread
            //Thread acceptThread = new Thread(new ThreadStart(Accept));
            //acceptThread.Start();
        }

        //private void Accept() 
        //{
        //    while (listening)
        //    {
        //        try
        //        {
        //            SimpleTcpClient newClient = server.Accept();
        //            socketList.Add(newClient); //accept clients
        //            logs.AppendText("A client is connected \n");

        //            Thread receiveThread = new Thread(() => Receive(newClient)); // updated
        //            receiveThread.Start();
        //        }
        //        catch
        //        {
        //            if (terminating)
        //            {
        //                listening = false;
        //            }
        //            else
        //            {
        //                logs.AppendText("The socket stopped working \n");
        //            }
        //        }
        //    }
        //}


        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            listening = false;
            btnListen.Enabled = true;
            btnStop.Enabled = false;
        }

    }
}
