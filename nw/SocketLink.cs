using System.Net.Sockets;
using System.Threading.Tasks;

namespace nodewire
{
    public class SocketLink: Link
    {
        TcpClient client;
        System.IO.StreamReader reader;
        System.IO.StreamWriter writer;

        public string server;
        public string account;
        public string pwd;
        public string instance;

        private bool failed = false;

        public SocketLink()
        {
            client = new TcpClient();
        }

        public async Task Connect()
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(server, 10001);
                NetworkStream stream = client.GetStream();
                reader = new System.IO.StreamReader(stream);
                writer = new System.IO.StreamWriter(stream);
                writer.AutoFlush = true;
            }

            failed = false;

            writer.WriteLine($"cp Gateway user={account} pwd={pwd} {instance}");
        }

        public void Disconnect()
        {
            client.Close();
        }

        public async override System.Threading.Tasks.Task<PlainMessage> Receive()
        {
            try
            {
                if(client.Connected)
                    return new PlainMessage(await reader.ReadLineAsync());
                else
                {
                    failed = true;
                    throw new System.IO.IOException();
                }
            }
            catch (System.NullReferenceException)
            {
                failed = true;
                throw new System.IO.IOException();
            }
            
        }

        public override bool connected()
        {
            return client.Connected && !failed;
        }

        public override void send(PlainMessage message)
        {
            if(client.Connected)
                writer.WriteLine(message.ToString());
            else
            {
                failed = true;
                throw new System.IO.IOException();
            }
        }
    }
}
