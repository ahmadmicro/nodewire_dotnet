using System.Windows.Forms;

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
}
