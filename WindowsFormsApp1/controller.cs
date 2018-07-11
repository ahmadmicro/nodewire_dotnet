using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class Controller
    {
        public RadioButton radio;
        public void On_led(dynamic p)
        {
            if (p == 1) radio.Checked = true; else radio.Checked = false;
        }
    }
}
