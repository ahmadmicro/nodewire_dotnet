using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using nodewire;

namespace Biometric
{
    public partial class Form1 : Form
    {
        SocketLink link;
        dynamic node;
        dynamic bio;

        public int refsize1;
        public int refsize2;
        public int refsize3;
        public byte[] refbuf1;
        public byte[] refbuf2;
        public byte[] refbuf3;

        public Form1()
        {
            InitializeComponent();
            refbuf1 = new byte[512];
            refbuf2 = new byte[512];
            refbuf3 = new byte[512];
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FormFingerEnrol ffe = new FormFingerEnrol();
            if (ffe.ShowDialog() == DialogResult.OK)
            {
                refsize1 = ffe.refsize;
                ffe.refbuf.CopyTo(refbuf1, 0);
                pictureBox1.Image = ffe.fingerbmp;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            FormFingerEnrol ffe = new FormFingerEnrol();
            if (ffe.ShowDialog() == DialogResult.OK)
            {
                refsize2 = ffe.refsize;
                ffe.refbuf.CopyTo(refbuf2, 0);
                pictureBox2.Image = ffe.fingerbmp;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            FormFingerEnrol ffe = new FormFingerEnrol();
            if(ffe.ShowDialog() == DialogResult.OK)
            {
                refsize3 = ffe.refsize;
                ffe.refbuf.CopyTo(refbuf3, 0);
                pictureBox3.Image = ffe.fingerbmp;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dynamic user = new JObject();
            user.username = textBox2.Text;
            user.userid = textBox1.Text;
            user.fp1 = Convert.ToBase64String(refbuf1, 0, refsize1);
            user.fp2 = Convert.ToBase64String(refbuf2,0, refsize2);
            user.fp3 = Convert.ToBase64String(refbuf3, 0, refsize3);
            user.enlcon1 = "AQAAAAA=";
            user.enlcon2 = "AQAAAAA=";
            user.enlcon3 = "AQAAAAA=";
            user.expdate = "AAAA";
            user.enllNO = "AAAAAA==";
            user.usertype = 1;
            user.groupid = 0;
 
            bio.add = user;
            MessageBox.Show("Submitted successfully");

            textBox1.Text = "";
            textBox2.Text = "";
            try
            {
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
            }
            catch { }

        }

        public void Connected()
        {
            bio = node.ConnectNode(comboBox1.Text);
            button2.Text = "connected";
            button3.Enabled = true;
        }

        public void Disconnected()
        {
            button2.Text = "connect";
            button2.Enabled = true;
            comboBox1.Enabled = true;
            button3.Enabled = false;
            button1.Enabled = false;
            button4.Enabled = false;
        }

        public void GotNode(dynamic remote)
        {
            if (remote.address == comboBox1.Text)
            {
                bio = remote;
                button2.Text = "==> " + remote.address;
                button1.Enabled = true;
                button4.Enabled = true;

                ((Node)node).When(comboBox1.Text + "." + "record", (PlainMessage msg) => {
                    Console.WriteLine(msg.Value);
                });
            }
            else if(remote.address == comboBox2.Text)
            {
                toolStripStatusLabel1.Text = "found " + comboBox2.Text;
                bio.rec_no = "all";
                
            }
        }

        public void On_led(dynamic p)
        {
            Console.WriteLine(p);
        }



        async private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "connecting...";
            button2.Enabled = false;
            comboBox1.Enabled = false;
            link = new SocketLink
            {
                server = "dashboard.nodewire.org",
                account = "test@microscale.net",
                pwd = "aB01@",
                instance = "1jex2k7cbedg"
            };
            try
            {
                await link.Connect();
                node = new Node("c#", link, this) {Inputs = "led" };
                node.led =  1;
                await node.Run();
            }
            catch
            {
                button2.Text = "connect";
                button1.Enabled = false;
                button4.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = false;
                comboBox1.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            link.Disconnect();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "looking for " + comboBox2.Text;
            dynamic target = node.ConnectNode(comboBox2.Text);
        }
    }
}
