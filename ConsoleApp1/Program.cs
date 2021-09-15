using System;
using nodewire;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
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
            var node03 = node.ConnectNode("node03");
            node.When("node03.2", (PlainMessage msg) =>
            {
                Console.WriteLine("changed");
            });
        }

        public void GotNode(dynamic node)
        {
            if (node.address == "node03")
                node["2"] = 1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SocketLink link = new SocketLink
            {
                server = "localhost",
                account = "sadiq.a.ahmad@gmail.com",
                pwd = "fupbvxvuce",
                instance = "9ie673ff3bei"
            };


            var c = link.Connect();
            c.Wait();

            Controller ctrl = new Controller();

            var node = new Node("node01", link, ctrl);
            ctrl.node = node;

            node.Inputs = "led";
            node.Outputs = "switch";
            var r = node.Run();
            r.Wait();
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
