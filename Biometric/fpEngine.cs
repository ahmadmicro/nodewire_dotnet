using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Biometric
{
    class fpengine
    {
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetMsgMainHandle(IntPtr hWnd);

        //Open Device
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenDevice(int nPortNum, int nPortPara, int nDeviceType);

        //Link Device
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LinkDevice();

        //Close Device
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseDevice();

        //Capture Fingerpint Image
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CaptureImage();

        //Capture Fingerprint Template
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void GenFpChar();

        //Enrol Fingerprint Template
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void EnrolFpChar();

        //Get Work Message
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetWorkMsg();

        //Get Result Message
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetRetMsg();

        //Release Message
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReleaseMsg();

        //Get Template By Capture
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFpCharByGen(byte[] pRefVal, ref int pRefSize);

        //Get Template By Enrol
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFpCharByEnl(byte[] pRefVal, ref int pRefSize);

        //Get Template By Capture (Ansi String)
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFpStrByGen(byte[] pRefVal);

        //Get Template By Enrol (Ansi String)
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFpStrByEnl(byte[] pRefVal);

        //Create Template Form Raw Image
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int CreateTemplate(byte[] pFingerData, byte[] pTemplate);

        //Match Fingerprint Template
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MatchTemplate(byte[] pSrcData, byte[] pDstData);

        //Match Fingerprint Template
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MatchTemplateOne(byte[] pSrcData, byte[] pDstData, int nDstSize);

        //Match Fingerprint Template (Ansi String)
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern int MatchTemplateEx(byte[] pSrcData, byte[] pDstData);
        public static extern int MatchTemplateEx(byte[] pDstData, byte[] pSrcData);

        //Get Fingerpirnt Image (RAW)
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetImage(byte[] imagedata, ref int size);

        //Set Fingerprint Image (RAW)
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetImage(byte[] imagedata, int size);

        //Draw Fingerpirnt Image
        [System.Runtime.InteropServices.DllImport("fpengine.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DrawImage(IntPtr hdc, int left, int top);


        public const int FPDATASIZE = 256;
        public const int IMGWIDTH = 256;
        public const int IMGHEIGHT = 288;
        public const int IMGSIZE = 73728;

        //Message
        public const int WM_FPMESSAGE = 1024 + 120; //Self Define Message
        public const int FPM_DEVICE = 0x01;			//Device Status
        public const int FPM_PLACE = 0x02;			//Please Place Finger
        public const int FPM_LIFT = 0x03;			//Please Lift Finger
        public const int FPM_CAPTURE = 0x04;		//Capture Image
        public const int FPM_ENROLL = 0x06;			//Enrol Template
        public const int FPM_GENCHAR = 0x05;        //Capture Template
        public const int FPM_NEWIMAGE = 0x07;		//Fingerprint Image
        public const int FPM_TIMEOUT = 0x08;        //Time Out
        public const int RET_OK = 0x1;
        public const int RET_FAIL = 0x0;

    }
}
