using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShow.BaseClasses;

namespace DesktopSource
{
    [ComVisible(true)]
    [Guid("BC7F9A0C-00DF-460F-A39E-DD9C9098411A")]
    public partial class DesktopSourcePropertyPage : BasePropertyPage
    {
        public DesktopSourcePropertyPage()
        {
            InitializeComponent();

            //InitializeCaptureMonitors();
            //InitializeCaptureWindows();
        }

        private void InitializeCaptureWindows()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    this.captureMethodCombo.Items.Add(process.ProcessName);
                }
            }
        }

        private void InitializeCaptureMonitors()
        {
            
        }
    }
}
