using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DirectShow;
using DirectShow.BaseClasses;
using SharpDX.Diagnostics;
using Sonic;

namespace DesktopSource
{

    [ComVisible(true)]
    [Guid("0160C224-D299-4EA0-8E2B-53A298E72909")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [System.Security.SuppressUnmanagedCodeSecurity]
    public interface IChangeCaptureSettings
    {
        [PreserveSig]
        HRESULT ChangeCaptureSettings(CaptureSettings newSettings);
    }

    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct CaptureSettings
    {
        public int m_Adapter { get; set; }
        public int m_Output { get; set; }
        public DsRect m_Rect { get; set; }
    }

    [ComVisible(true)]
    [Guid("3741E463-EE30-4301-B9C8-A0685F9781C6")]
    [AMovieSetup(Merit.Normal, AMovieSetup.CLSID_VideoInputDeviceCategory)]
    [PropPageSetup(typeof(DesktopSourcePropertyPage))]
    public class DesktopSource : BaseSourceFilter, IChangeCaptureSettings
    {

        public DesktopSource() : base("Desktop Source")
        {
        }

        public HRESULT ChangeCaptureSettings(CaptureSettings newSettings)
        {
            if (IsActive) return VFW_E_WRONG_STATE;

            return ((DesktopStream) Pins[0]).ChangeCaptureSettings(newSettings);
        }

        protected override int OnInitializePins()
        {
            AddPin(new DesktopStream("Output", this));

            return NOERROR;
        }
    }
}