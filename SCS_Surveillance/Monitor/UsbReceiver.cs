using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS.Surveillance.Monitor
{
    public partial class UsbReceiver : Form
    {
        public event Action newUsbDetect;

        private const int WM_DEVICECHANGE = 0x219;
        private readonly IntPtr NEW_USB = new IntPtr(32768);

        private readonly IntPtr blockStateNewUsb = new IntPtr(24);
        private readonly IntPtr blockStateNewUsbResult = new IntPtr(11);

        public UsbReceiver()
        {
            InitializeComponent();
            Opacity = 0;
            Shown += UsbReceiver_Shown;
        }

        private void UsbReceiver_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.WParam == blockStateNewUsb && m.Result == blockStateNewUsbResult)
            {
                newUsbDetect?.Invoke();
            }
            //MessageBox.Show(m.LParam + ", " + m.WParam + ", " + m.Result);
            if (m.Msg == WM_DEVICECHANGE && m.WParam == NEW_USB)
            {
                newUsbDetect?.Invoke();
            }
        }
    }
}
