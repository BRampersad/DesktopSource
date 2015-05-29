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
using DirectShow;
using DirectShow.BaseClasses;
using SharpDX.DXGI;
using Sonic;

namespace DesktopSource
{
    [ComVisible(true)]
    [Guid("BC7F9A0C-00DF-460F-A39E-DD9C9098411A")]
    public partial class DesktopSourcePropertyPage : BasePropertyPage
    {
        public IChangeCaptureSettings m_FilterSettings { get; set; }

        private class CaptureItem
        {
            public string m_Name { get; set; }
            public CaptureSettings m_CaptureSettings { get; set; }

            public CaptureItem(string name, CaptureSettings settings)
            {
                m_Name = name;
                m_CaptureSettings = settings;
            }

            public override string ToString()
            {
                return m_Name;
            }
        }

        public DesktopSourcePropertyPage()
        {
            InitializeComponent();

            captureMethodCombo.Items.Clear();
            InitializeCaptureMonitors();
            InitializeCaptureWindows();
        }

        private void InitializeCaptureWindows()
        {
            EnumWindows(EnumWindows, IntPtr.Zero);
        }

        private void InitializeCaptureMonitors()
        {

            var factory = new Factory1();

            for (int i = 0; i < factory.GetAdapterCount(); i++)
            {
                for (int j = 0; j < factory.GetAdapter(i).GetOutputCount(); j++)
                {
                    var output = factory.GetAdapter(i).GetOutput(j);

                    CaptureSettings settings = new CaptureSettings();
                    settings.m_Adapter = i;
                    settings.m_Output = j;
                    settings.m_Rect = new DsRect(
                        output.Description.DesktopBounds.Left,
                        output.Description.DesktopBounds.Top,
                        output.Description.DesktopBounds.Right,
                        output.Description.DesktopBounds.Bottom
                    );

                    CaptureItem captureItem = new CaptureItem(output.Description.DeviceName, settings);
                    captureMethodCombo.Items.Add(captureItem);
                }
            }
        }

        protected bool EnumWindows(IntPtr hWnd, IntPtr lParam)
        {
            int size = GetWindowTextLength(hWnd);

            if (size++ > 0 && IsWindowVisible(hWnd))
            {
                StringBuilder sb = new StringBuilder(size);
                GetWindowText(hWnd, sb, size);

                CaptureSettings captureSettings = new CaptureSettings
                {
                    m_Adapter = 0,
                    m_Output = 0
                };

                RECT rct;

                GetWindowRect(hWnd, out rct);

                captureSettings.m_Rect = new DsRect(rct.Left, rct.Top, rct.Right, rct.Bottom);

                CaptureItem captureItem = new CaptureItem(sb.ToString(), captureSettings);

                captureMethodCombo.Items.Add(captureItem);
            }

            return true; 
        }

        public override HRESULT OnConnect(IntPtr pUnknown)
        {
            m_FilterSettings = (IChangeCaptureSettings) Marshal.GetObjectForIUnknown(pUnknown);

            return HRESULT.NOERROR;
        }

        public override HRESULT OnDisconnect()
        {
            Marshal.ReleaseComObject(m_FilterSettings);

            return HRESULT.NOERROR;
        }

        public override HRESULT OnApplyChanges()
        {
            if (m_FilterSettings != null && captureMethodCombo.SelectedItem != null)
            {
                CaptureItem setting = (captureMethodCombo.SelectedItem as CaptureItem);
                if (setting != null) return m_FilterSettings.ChangeCaptureSettings(setting.m_CaptureSettings);
            }

            return HRESULT.NOERROR;
        }

        #region API
        protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)] 
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount); 
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] 
        protected static extern int GetWindowTextLength(IntPtr hWnd); 
        [DllImport("user32.dll")] 
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam); 
        [DllImport("user32.dll")] 
        protected static extern bool IsWindowVisible(IntPtr hWnd); 
        #endregion

        private void captureMethodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CaptureSettings newSettings = ((CaptureItem) captureMethodCombo.SelectedItem).m_CaptureSettings;
            adapterTxtBox.Text = newSettings.m_Adapter.ToString();
            outputTxtBox.Text = newSettings.m_Output.ToString();
            topTextBox.Text = newSettings.m_Rect.top.ToString();
            leftTextBox.Text = newSettings.m_Rect.left.ToString();
            rightTextBox.Text = newSettings.m_Rect.right.ToString();
            bottomTextBox.Text = newSettings.m_Rect.bottom.ToString();

            this.Dirty = true;
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            captureMethodCombo.Items.Clear();

            InitializeCaptureMonitors();
            InitializeCaptureWindows();
        }
    }
}
