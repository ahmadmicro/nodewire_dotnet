using System;
using nodewire;
using Newtonsoft.Json.Linq;

namespace ConsoleApp2
{
    class Controller
    {
        public Node node = null;
        public void On_led(dynamic p)
        {
            Console.WriteLine("LED on");
        }

        public dynamic Get_switch()
        {
            return 1;
        }

        public void Connected()
        {
            var node03 = node.ConnectNode("seismic");
            node.When("seismic.status", (PlainMessage msg) =>
            {
                Console.WriteLine(msg);
            });
        }

        public void GotNode(dynamic node)
        {
            if (node.address == "seismic")
            {
                dynamic sensor = new JObject();
                sensor.id = 1;
                sensor.sid = 2;
                node.sensor = sensor;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SocketLink link = new SocketLink
            {
                server = "cloud.nodewire.org",
                account = "sadiq.a.ahmad@gmail.com",
                pwd = "secret",
                instance = "lrsnr49yxurz"
            };


            var c = link.connect();
            c.Wait();

            Controller ctrl = new Controller();

            var node = new Node("node01", link, ctrl);
            ctrl.node = node;

            node.Inputs = "led";
            node.Outputs = "switch";
            node.Run();
            Console.ReadLine();
        }


        static void Test_PlainMessage()
        {
            dynamic user = new JObject();
            user.name = "Ahmad Sadiq";
            user.age = 9;

            PlainMessage message = new PlainMessage(@"cp portvalue user {'name':'ahmad sadiq', 'age':35} node01");
            PlainMessage msg = new PlainMessage { address = "cp", command = "ThisIs", sender = "node01" };
            PlainMessage cmd = new PlainMessage { address = "node01", command = "set", Port = "add", Value = user, sender = "ah" };
        }
    }
}
