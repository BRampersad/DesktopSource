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
using Resource = SharpDX.Direct3D11.Resource;

namespace DesktopSource
{

    [ComVisible(true)]
    [Guid("0160C224-D299-4EA0-8E2B-53A298E72909")]
    public interface IChangeCaptureSettings
    {
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
        public int m_nWidth { get; private set; }
        public int m_nHeight { get; private set; }
        public long m_nAvgTimePerFrame { get; private set; }
        public long m_lLastSampleTime { get; private set; }
        public CaptureSettings m_CaptureSettings { get; set; }

        protected int m_NumAdapter;
        protected int m_NumOutput;
        protected Factory1 m_Factory;
        protected Adapter1 m_Adapter;
        protected Device m_Device;
        protected Output1 m_Output;
        protected Texture2D m_ScreenTexture;
        protected OutputDuplication m_DuplicatedOutput;

        public DesktopSource() : base("Desktop Source")
        {
            m_nAvgTimePerFrame = UNITS / 30;

            CaptureSettings defaultSettings = new CaptureSettings();
            defaultSettings.m_Adapter = 0;
            defaultSettings.m_Output = 0;
            defaultSettings.m_Rect = new DsRect(Screen.AllScreens[0].Bounds);

            ChangeCaptureSettings(defaultSettings);
        }

        public HRESULT ChangeCaptureSettings(CaptureSettings newSettings)
        {
            Dispose();

            m_NumAdapter = newSettings.m_Adapter;
            m_NumOutput = newSettings.m_Output;
            m_nWidth = Math.Abs(newSettings.m_Rect.left - newSettings.m_Rect.right);
            m_nHeight = Math.Abs(newSettings.m_Rect.top - newSettings.m_Rect.bottom);
            m_CaptureSettings = newSettings;

            m_Factory = new Factory1();
            m_Adapter = m_Factory.GetAdapter1(m_NumAdapter);
            m_Device = new Device(m_Adapter);

            Output output = m_Adapter.GetOutput(m_NumOutput);
            m_Output = output.QueryInterface<Output1>();

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

            m_ScreenTexture = new Texture2D(m_Device, textureDesc);

            m_DuplicatedOutput = m_Output.DuplicateOutput(m_Device);

            AMMediaType am = new AMMediaType();
            GetMediaType(ref am);

            ((DesktopStream) Pins[0]).SetFormat(am);

            return S_OK;
        }

        ~DesktopSource()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_Factory != null) m_Factory.Dispose();
            if (m_Adapter != null) m_Adapter.Dispose();
            if (m_Device != null) m_Device.Dispose();
            if (m_Output != null) m_Output.Dispose();
            if (m_ScreenTexture != null) m_ScreenTexture.Dispose();
            if (m_DuplicatedOutput != null) m_DuplicatedOutput.Dispose();
        }

        protected override int OnInitializePins()
        {
            AddPin(new DesktopStream("Output", this));

            return NOERROR;
        }

        public int GetMediaType(ref AMMediaType pMediaType)
        {
            pMediaType.majorType = DirectShow.MediaType.Video;
            pMediaType.subType = DirectShow.MediaSubType.RGB32;
            pMediaType.formatType = DirectShow.FormatType.VideoInfo;

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



        public int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            AllocatorProperties _actual = new AllocatorProperties();

            BitmapInfoHeader _bmi = Pins[0].CurrentMediaType;
            prop.cbBuffer = _bmi.GetBitmapSize();
            if (prop.cbBuffer < _bmi.ImageSize)
            {
                prop.cbBuffer = _bmi.ImageSize;
            }
            prop.cBuffers = 1;

            int hr = pAlloc.SetProperties(prop, _actual);
            return hr;
        }

        public int FillBuffer(ref IMediaSampleImpl pSample)
        {
            SharpDX.DXGI.Resource screenResource;
            OutputDuplicateFrameInformation duplicateFrameInformation;

            m_DuplicatedOutput.AcquireNextFrame(10000, out duplicateFrameInformation, out screenResource);

            ResourceRegion region = new ResourceRegion(
                m_CaptureSettings.m_Rect.left,
                m_CaptureSettings.m_Rect.top,
                0,
                m_CaptureSettings.m_Rect.right,
                m_CaptureSettings.m_Rect.bottom,
                1
            );

            var screenTextureAsResource = m_ScreenTexture.QueryInterface<Resource>();

            m_Device.ImmediateContext.CopySubresourceRegion(screenResource.QueryInterface<Resource>(), 0, region, screenTextureAsResource, 0, 0, 0, 0);

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


            pSample.SetActualDataLength(Pins[0].CurrentMediaType.sampleSize);
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