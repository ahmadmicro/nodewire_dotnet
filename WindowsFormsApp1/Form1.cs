using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using nodewire;

namespace WindowsFormsApp1
{
    class controller
    {
        public RadioButton radio;
        public void on_led(dynamic p)
        {
            if (p == 1) radio.Checked = true; else radio.Checked = false;
        }
    }
    public partial class Form1 : Form
    {
        SocketLink link;
        Node node;

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            link = new SocketLink {
                server = "dashboard.nodewire.org",
                account = "sadiq.a.ahmad@gmail.com",
                pwd = "secret",
                instance = "lrsnr49yxurz"
            };
            await link.connect();
            controller ctrl = new controller { radio = radioButton1 };
            node = new Node("c#", link, ctrl) { Inputs = "led" };
            await node.Run();
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {


        }
    }
}
