using System;
using System.Runtime.InteropServices;
using DirectShow;
using DirectShow.BaseClasses;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Sonic;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

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
        public int m_Width { get; set; }
        public int m_Height { get; set; }
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

        private const int m_NumAdapter = 0;
        private const int m_NumOutput = 0;
        protected Factory1 m_Factory;
        protected Adapter1 m_Adapter;
        protected Device m_Device;
        protected Output1 m_Output;
        protected Texture2D m_ScreenTexture;
        protected OutputDuplication m_DuplicatedOutput;

        public DesktopSource() : base("Desktop Source")
        {
            m_Factory = new Factory1();
            m_Adapter = m_Factory.GetAdapter1(m_NumAdapter);
            m_Device = new Device(m_Adapter);

            Output output = m_Adapter.GetOutput(m_NumOutput);
            m_Output = output.QueryInterface<Output1>();

            m_nWidth = Math.Abs(output.Description.DesktopBounds.Right - output.Description.DesktopBounds.Left);
            m_nHeight = Math.Abs(output.Description.DesktopBounds.Top - output.Description.DesktopBounds.Bottom);
            m_nAvgTimePerFrame = UNITS / 30;

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
        }

        public HRESULT ChangeCaptureSettings(CaptureSettings newSettings)
        {
            Dispose();

            m_Factory = new Factory1();
            m_Adapter = m_Factory.GetAdapter1(newSettings.m_Adapter);
            m_Device = new Device(m_Adapter);

            Output output = m_Adapter.GetOutput(newSettings.m_Output);
            m_Output = output.QueryInterface<Output1>();

            m_nWidth = newSettings.m_Width;
            m_nHeight = newSettings.m_Height;
            m_nAvgTimePerFrame = UNITS / 30;

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


            return S_OK;
        }

        ~DesktopSource()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_Factory.Dispose();
            m_Adapter.Dispose();
            m_Device.Dispose();
            m_Output.Dispose();
            m_ScreenTexture.Dispose();
            m_DuplicatedOutput.Dispose();
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

            using (Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>())
            {
                m_Device.ImmediateContext.CopyResource(screenTexture2D, m_ScreenTexture);
            }

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