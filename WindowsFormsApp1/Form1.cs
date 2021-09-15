using System;
using System.Windows.Forms;
using nodewire;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SocketLink link;
        Node node;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            link = new SocketLink
            {
                server = "dashboard.nodewire.org",
                account = "sadiq.a.ahmad@gmail.com",
                pwd = "secret",
                instance = "lrsnr49yxurz"
            };
            await link.Connect();
            Controller ctrl = new Controller { radio = radioButton1 };
            node = new Node("c#", link, ctrl) { Inputs = "led" };
            await node.Run();
        }
    }
}
