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

        public SocketLink()
        {
            client = new TcpClient();
        }

        public async Task connect()
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(server, 10001);
                NetworkStream stream = client.GetStream();
                reader = new System.IO.StreamReader(stream);
                writer = new System.IO.StreamWriter(stream);
                writer.AutoFlush = true;
            }

            writer.WriteLine($"cp Gateway user={account} pwd={pwd} {instance}");
        }

        public async override System.Threading.Tasks.Task<PlainMessage> Receive()
        {
            return new PlainMessage(await reader.ReadLineAsync());
        }

        public override void send(PlainMessage message)
        {
            writer.WriteLine(message.ToString());
        }
    }
}
