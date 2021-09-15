using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;

namespace nodewire
{
    class ServerSocketLink: Link
    {
        TcpListener listener;

        public string server;
        public string account;
        public string pwd;
        public string instance;

        private List<dynamic> Nodes = new List<dynamic>();

        public ServerSocketLink()
        {
           
        }

        public void Connect()
        {
            IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            listener = new TcpListener(ipAddress, 10001);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
        }

        private void OnClientConnected(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient clientSocket = listener.EndAcceptTcpClient(asyncResult);
                if (clientSocket != null)
                    Console.WriteLine("Received connection request from: " + clientSocket.Client.RemoteEndPoint.ToString());
                HandleClientRequest(clientSocket);
            }
            catch
            {
                throw;
            }
            listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
        }

        private void HandleClientRequest(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            System.IO.StreamReader reader;
            System.IO.StreamWriter writer;
            reader = new System.IO.StreamReader(stream);
            writer = new System.IO.StreamWriter(stream);
            writer.AutoFlush = true;

            dynamic entry = new ExpandoObject();
            entry.reader = reader;
            entry.writer = writer;

            var m = reader.ReadLineAsync();
            m.Wait();
            PlainMessage msg = new PlainMessage(m.Result);

            entry.address = msg.sender;

            Nodes.Add(entry);
        }

       public async override Task<PlainMessage> Receive()
        {
            var readers = from node in Nodes select node.reader.ReadLineAsync();
            Task<String> result = await Task.WhenAny((IEnumerable<Task<String>>)readers);
            return new PlainMessage(result.Result);
            //return new PlainMessage(await reader.ReadLineAsync());
        }

        public override void send(PlainMessage message)
        {
            var n = from node in Nodes where node.address == message.address select node;
            if (n.Count() != 0)
                n.First().writer.WriteLine(message.ToString());
            else
                Console.WriteLine($"Node with address '{message.address}' not found");
        }
    }
}
