using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DirectShow;
using DirectShow.BaseClasses;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Sonic;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.DXGI.Resource;


/// <summary>
/// The Main DesktopSource Namespace
/// </summary>
namespace DesktopSource
{

    /// <summary>
    /// An OutputPin that Streams a Monitor or a subset of a Monitor.
    /// </summary>
    [ComVisible(true)]
    [Guid("47A3CC6B-887D-4DEA-9654-002D1190EBC7")]
    public class DesktopStream : SourceStream
    {

        /// <summary>
        /// Gets the width of the capture region.
        /// </summary>
        /// <value>
        /// The width of the capture region.
        /// </value>
        public int m_nWidth
        {
            get { return Math.Abs(m_CaptureSettings.m_Rect.left - m_CaptureSettings.m_Rect.right); }
        }



        /// <summary>
        /// Gets the height of the capture region.
        /// </summary>
        /// <value>
        /// The height of the capture region.
        /// </value>
        public int m_nHeight
        {
            get { return Math.Abs(m_CaptureSettings.m_Rect.top - m_CaptureSettings.m_Rect.bottom); }
        }


        /// <summary>
        /// Gets the average time per frame.
        /// </summary>
        /// <value>
        /// The average time per frame.
        /// </value>
        public long m_nAvgTimePerFrame { get; private set; }


        /// <summary>
        /// Gets the last sample time.
        /// </summary>
        /// <value>
        /// The last sample time.
        /// </value>
        public long m_lLastSampleTime { get; private set; }


        /// <summary>
        /// Gets the current capture settings.
        /// </summary>
        /// <value>
        /// The capture settings.
        /// </value>
        public CaptureSettings m_CaptureSettings { get; private set; }


        /// <summary>
        /// The ShatpDX factory
        /// </summary>
        protected Factory1 m_Factory;


        /// <summary>
        /// The current video adapter
        /// </summary>
        protected Adapter1 m_Adapter;


        /// <summary>
        /// The current GPU device
        /// </summary>
        protected Device m_Device;


        /// <summary>
        /// The current display output
        /// </summary>
        protected Output1 m_Output;


        /// <summary>
        /// The m_ screen texture
        /// </summary>
        protected Texture2D m_ScreenTexture;


        /// <summary>
        /// The m_ duplicated output
        /// </summary>
        protected OutputDuplication m_DuplicatedOutput;


        /// <summary>
        /// The m_ default settings
        /// </summary>
        protected readonly CaptureSettings m_DefaultSettings = new CaptureSettings
        {
            m_Adapter = 0,
            m_Output = 0,
            m_Rect = new DsRect(Screen.AllScreens[0].Bounds)
        };


        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopStream"/> class.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <param name="_filter">The _filter.</param>
        public DesktopStream(string _name, BaseSourceFilter _filter) : base(_name, _filter)
        {
            m_Factory = new Factory1();

            m_nAvgTimePerFrame = UNITS / 30; // 30 FPS 
            m_lLastSampleTime = 0L;

            m_CaptureSettings = m_DefaultSettings;

            ChangeCaptureSettings(m_CaptureSettings);
        }


        /// <summary>
        /// Finalizes an instance of the <see cref="DesktopStream"/> class.
        /// </summary>
        ~DesktopStream()
        {
            if (m_Factory != null) m_Factory.Dispose();
            if (m_Adapter != null) m_Adapter.Dispose();
            if (m_Device != null) m_Device.Dispose();
            if (m_Output != null) m_Output.Dispose();
            if (m_ScreenTexture != null) m_ScreenTexture.Dispose();
            if (m_DuplicatedOutput != null) m_DuplicatedOutput.Dispose();
        }


        /// <summary>
        /// Changes the capture settings.
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        /// <returns></returns>
        public HRESULT ChangeCaptureSettings(CaptureSettings newSettings)
        {
            lock (m_Filter.FilterLock)
            {
                if (m_Filter.IsActive)
                {
                    return VFW_E_WRONG_STATE;
                }

                m_CaptureSettings = newSettings;

                if (m_Adapter != null) m_Adapter.Dispose();
                m_Adapter = m_Factory.GetAdapter1(m_CaptureSettings.m_Adapter);

                if (m_Adapter == null) return E_POINTER;

                if (m_Device != null) m_Device.Dispose();
                m_Device = new Device(m_Adapter);

                if (m_Device == null) return E_POINTER;
                
                if (m_Output != null) m_Output.Dispose();
                m_Output = m_Adapter.GetOutput(m_CaptureSettings.m_Output).QueryInterface<Output1>();

                if (m_Output == null) return E_POINTER;

                Texture2DDescription textureDesc = new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = m_nWidth,
                    Height = m_nHeight,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };

                if (m_ScreenTexture != null) m_ScreenTexture.Dispose();
                m_ScreenTexture = new Texture2D(m_Device, textureDesc);

                if (m_ScreenTexture == null) return E_POINTER;

                if (m_DuplicatedOutput != null) m_DuplicatedOutput.Dispose();
                m_DuplicatedOutput = m_Output.DuplicateOutput(m_Device);

                if (m_DuplicatedOutput == null) return E_POINTER;

                return (HRESULT)ReconnectPin();
            }
        }


        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <param name="pMediaType">Type of the p media.</param>
        /// <returns></returns>
        public override int GetMediaType(ref AMMediaType pMediaType)
        {
            pMediaType.majorType = MediaType.Video;
            pMediaType.subType = MediaSubType.RGB32;
            pMediaType.formatType = FormatType.VideoInfo;

            VideoInfoHeader vih = new VideoInfoHeader();
            vih.AvgTimePerFrame = m_nAvgTimePerFrame;
            vih.BmiHeader = new BitmapInfoHeader();
            vih.BmiHeader.Size = Marshal.SizeOf(typeof(BitmapInfoHeader));
            vih.BmiHeader.Compression = 0;
            vih.BmiHeader.BitCount = 32;
            vih.BmiHeader.Width = m_nWidth;
            vih.BmiHeader.Height = m_nHeight;
            vih.BmiHeader.Planes = 1;
            vih.BmiHeader.ImageSize = vih.BmiHeader.Width * vih.BmiHeader.Height * vih.BmiHeader.BitCount / 8;
            vih.SrcRect = new DsRect();
            vih.TargetRect = new DsRect();

            AMMediaType.SetFormat(ref pMediaType, ref vih);
            pMediaType.fixedSizeSamples = true;
            pMediaType.sampleSize = vih.BmiHeader.ImageSize;

            return NOERROR;
        }


        /// <summary>
        /// Decides the size of the buffer.
        /// </summary>
        /// <param name="pAlloc">The p alloc.</param>
        /// <param name="prop">The property.</param>
        /// <returns></returns>
        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            AllocatorProperties _actual = new AllocatorProperties();

            BitmapInfoHeader _bmi = CurrentMediaType;
            prop.cbBuffer = _bmi.GetBitmapSize();
            if (prop.cbBuffer < _bmi.ImageSize)
            {
                prop.cbBuffer = _bmi.ImageSize;
            }
            prop.cBuffers = 1;

            int hr = pAlloc.SetProperties(prop, _actual);
            return hr;
        }


        /// <summary>
        /// Fills the buffer.
        /// </summary>
        /// <param name="pSample">The p sample.</param>
        /// <returns></returns>
        public override int FillBuffer(ref IMediaSampleImpl pSample)
        {
            Resource screenResource;
            OutputDuplicateFrameInformation duplicateFrameInformation;

            m_DuplicatedOutput.AcquireNextFrame(10000, out duplicateFrameInformation, out screenResource);

            ResourceRegion region = new ResourceRegion(
                Math.Max(m_CaptureSettings.m_Rect.left, 0),
                Math.Max(m_CaptureSettings.m_Rect.top, 0),
                0,
                Math.Min(m_CaptureSettings.m_Rect.right, m_Output.Description.DesktopBounds.Right),
                Math.Min(m_CaptureSettings.m_Rect.bottom, m_Output.Description.DesktopBounds.Bottom),
                1
            );

            m_Device.ImmediateContext.CopySubresourceRegion(
                screenResource.QueryInterface<SharpDX.Direct3D11.Resource>(), 
                0,
                region, 
                m_ScreenTexture.QueryInterface<SharpDX.Direct3D11.Resource>(), 
                0
            );

            DataBox mapSource = m_Device.ImmediateContext.MapSubresource(m_ScreenTexture, 0, MapMode.Read, MapFlags.None);

            IntPtr pImageDest;
            pSample.GetPointer(out pImageDest);


            var sourcePtr = mapSource.DataPointer;
            var destPtr = IntPtr.Add(pImageDest, (m_nHeight - 1) * m_nWidth * 4);

            for (int y = 0; y < m_nHeight; y++)
            {
                Utilities.CopyMemory(destPtr, sourcePtr, m_nWidth * 4);

                sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                destPtr = IntPtr.Subtract(destPtr, m_nWidth * 4);
            }


            pSample.SetActualDataLength(CurrentMediaType.sampleSize);
            pSample.SetSyncPoint(true);

            long _stop = m_lLastSampleTime + m_nAvgTimePerFrame;
            pSample.SetTime(m_lLastSampleTime, _stop);

            m_lLastSampleTime = _stop;

            m_Device.ImmediateContext.UnmapSubresource(m_ScreenTexture, 0);
            screenResource.Dispose();
            m_DuplicatedOutput.ReleaseFrame();

            return NOERROR;
        }
    }
}
