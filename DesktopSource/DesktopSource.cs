using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DirectShow;
using DirectShow.BaseClasses;
using SharpDX.Diagnostics;
using Sonic;


/// <summary>
/// 
/// </summary>
namespace DesktopSource
{

    /// <summary>
    /// 
    /// </summary>
    [ComVisible(true)]
    [Guid("0160C224-D299-4EA0-8E2B-53A298E72909")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [System.Security.SuppressUnmanagedCodeSecurity]
    public interface IChangeCaptureSettings
    {

        /// <summary>
        /// Changes the capture settings.
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        /// <returns></returns>
        [PreserveSig]
        HRESULT ChangeCaptureSettings([In] CaptureSettings newSettings);
    }


    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CaptureSettings
    {

        /// <summary>
        /// Gets or sets the m_ adapter.
        /// </summary>
        /// <value>
        /// The m_ adapter.
        /// </value>
        public int m_Adapter { get; set; }


        /// <summary>
        /// Gets or sets the m_ output.
        /// </summary>
        /// <value>
        /// The m_ output.
        /// </value>
        public int m_Output { get; set; }


        /// <summary>
        /// Gets or sets the m_ rect.
        /// </summary>
        /// <value>
        /// The m_ rect.
        /// </value>
        public DsRect m_Rect { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    [ComVisible(true)]
    [Guid("3741E463-EE30-4301-B9C8-A0685F9781C6")]
    [AMovieSetup(Merit.Normal, AMovieSetup.CLSID_VideoInputDeviceCategory)]
    [PropPageSetup(typeof(DesktopSourcePropertyPage))]
    public class DesktopSource : BaseSourceFilter, IChangeCaptureSettings
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopSource"/> class.
        /// </summary>
        public DesktopSource() : base("Desktop Source")
        {
        }


        /// <summary>
        /// Changes the capture settings.
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        /// <returns></returns>
        public HRESULT ChangeCaptureSettings(CaptureSettings newSettings)
        {
            return ((DesktopStream) Pins[0]).ChangeCaptureSettings(newSettings);
        }


        /// <summary>
        /// Called when [initialize pins].
        /// </summary>
        /// <returns></returns>
        protected override int OnInitializePins()
        {
            AddPin(new DesktopStream("Output", this));

            return NOERROR;
        }
    }
}