using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Biometric
{
    public partial class FormFingerEnrol : Form
    {
        //binary
        public int refsize;
        public byte[] refbuf;
        public System.Drawing.Bitmap fingerbmp;

        public FormFingerEnrol()
        {
            InitializeComponent();

            refsize = 0;
            refbuf = new byte[512];
            fingerbmp = new Bitmap(255, 288);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormFingerEnrol_Load(object sender, EventArgs e)
        {
            buttonOK.Enabled = false;
            timerFinger.Enabled = false;
            try
            {
                fpengine.CloseDevice();
            }
            catch
            {

            }
            
            if (fpengine.OpenDevice(0, 0, 0) == 1)
            {
                if (fpengine.LinkDevice() == 1)
                {
                    labelStatus.Text = "Open Fingerprint Reader Succeed";
                    //fpengine.SetTimeOut(30.0);
                    fpengine.EnrolFpChar();
                    timerFinger.Enabled = true;
                }
                else
                {
                    labelStatus.Text = "Link Fingerprint Reader Fail";
                }
            }
            else
            {
                labelStatus.Text = "Open Fingerprint Reader Fail";
            }
        }

        private void timerFinger_Tick(object sender, EventArgs e)
        {
            int wm = fpengine.GetWorkMsg();
            int rm = fpengine.GetRetMsg();
            switch (wm)
            {
                case fpengine.FPM_DEVICE:
                    labelStatus.Text = "Not Open Reader";
                    break;
                case fpengine.FPM_PLACE:
                    labelStatus.Text = "Please Plase Finger";
                    break;
                case fpengine.FPM_LIFT:
                    labelStatus.Text = "Please Lift Finger";
                    break;
                case fpengine.FPM_ENROLL:
                    {
                        if (rm == 1)
                        {
                            labelStatus.Text = "Enrol Fingerprint Template Succeed";
                            fpengine.GetFpCharByEnl(refbuf, ref refsize);
                            buttonOK.Enabled = true;
                        }
                        else
                        {
                            labelStatus.Text = "Enrol Fingerprint Template Fail";
                        }
                        timerFinger.Enabled = false;
                    }
                    break;
                case fpengine.FPM_NEWIMAGE:
                    {
                        //System.Drawing.Bitmap fingerbmp = new Bitmap(255, 288);                        
                        Graphics g = Graphics.FromImage(fingerbmp);
                        fpengine.DrawImage(g.GetHdc(), 0, 0);
                        g.Dispose();
                        pictureBoxFinger.Image = fingerbmp;
                    }
                    break;
                case fpengine.FPM_TIMEOUT:
                    {
                        labelStatus.Text = "Time Out";
                    }
                    break;
            }
        }

        private void FormFingerEnrol_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerFinger.Enabled = false;
            fpengine.CloseDevice();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (refsize > 0)
            {
                //buttonOK.DialogResult = DialogResult.OK;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void pictureBoxFinger_Click(object sender, EventArgs e)
        {

        }
    }
}