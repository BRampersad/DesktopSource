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

namespace DesktopSource
{
    // Output Pin (STreams the desktop frames)
    [ComVisible(true)]
    [Guid("47A3CC6B-887D-4DEA-9654-002D1190EBC7")]
    public class DesktopStream : SourceStream, IAMStreamConfig
    {
        public int m_nWidth
        {
            get { return Math.Abs(m_CaptureSettings.m_Rect.left - m_CaptureSettings.m_Rect.right); }
        }

        public int m_nHeight
        {
            get { return Math.Abs(m_CaptureSettings.m_Rect.top - m_CaptureSettings.m_Rect.bottom); }
        }

        public long m_nAvgTimePerFrame { get; private set; }

        public long m_lLastSampleTime { get; private set; }

        public CaptureSettings m_CaptureSettings { get; set; }

        protected Factory1 m_Factory;

        protected Adapter1 m_Adapter;

        protected Device m_Device;

        protected Output1 m_Output;

        protected Texture2D m_ScreenTexture;

        protected OutputDuplication m_DuplicatedOutput;

        public DesktopStream(string _name, BaseSourceFilter _filter) : base(_name, _filter)
        {
            m_Factory = new Factory1();

            m_nAvgTimePerFrame = UNITS / 30;
            m_lLastSampleTime = 0L;

            m_CaptureSettings = new CaptureSettings
            {
                m_Adapter = 0,
                m_Output = 0,
                m_Rect = new DsRect(Screen.AllScreens[0].Bounds)
            };
        }

        public override int Inactive()
        {
            lock (m_Filter.FilterLock)
            {
                m_Adapter.Dispose();
                m_Adapter = null;

                m_Device.Dispose();
                m_Device = null;

                m_Output.Dispose();
                m_Output = null;

                m_ScreenTexture.Dispose();
                m_ScreenTexture = null;

                m_DuplicatedOutput.Dispose();
                m_DuplicatedOutput = null;

                return base.Inactive();
            }
        }

        public override int Active()
        {
            lock(m_Filter.FilterLock)
            {
                ChangeCaptureSettings(m_CaptureSettings);

                return base.Active();
            }
        }

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

                if (m_Device != null) m_Device.Dispose();
                m_Device = new Device(m_Adapter);

                if (m_Output != null) m_Output.Dispose();
                m_Output = m_Adapter.GetOutput(m_CaptureSettings.m_Output).QueryInterface<Output1>();

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

                if (m_DuplicatedOutput != null) m_DuplicatedOutput.Dispose();
                m_DuplicatedOutput = m_Output.DuplicateOutput(m_Device);

                AMMediaType am = new AMMediaType();
                GetMediaType(ref am);

                return (HRESULT)SetFormat(am);
            }
        }

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

        public int SetFormat(AMMediaType pmt)
        {
            if (m_Filter.IsActive) return VFW_E_WRONG_STATE;

            HRESULT hr;
            AMMediaType _newType = new AMMediaType(pmt);
            AMMediaType _oldType = new AMMediaType(m_mt);

            hr = (HRESULT)CheckMediaType(_newType);

            if (FAILED(hr)) return hr;

            m_mt.Set(_newType);

            if (IsConnected)
            {
                hr = (HRESULT)Connected.QueryAccept(_newType);
                if (SUCCEEDED(hr))
                {
                    hr = (HRESULT)m_Filter.ReconnectPin(this, _newType);
                    if (SUCCEEDED(hr))
                    {
                        hr = (HRESULT)SetMediaType(_newType);
                    }
                    else
                    {
                        m_mt.Set(_oldType);
                        m_Filter.ReconnectPin(this, _oldType);
                    }
                }
            }
            else
            {
                hr = (HRESULT)SetMediaType(_newType);
            }

            return hr;
        }

        public int GetFormat(out AMMediaType pmt)
        {
            pmt = new AMMediaType(m_mt);

            return NOERROR;
        }

        public int GetNumberOfCapabilities(IntPtr piCount, IntPtr piSize)
        {
            if (piCount != IntPtr.Zero)
            {
                Marshal.WriteInt32(piCount, 1);
            }

            if (piSize != IntPtr.Zero)
            {
                Marshal.WriteInt32(piSize, Marshal.SizeOf(typeof(VideoStreamConfigCaps)));
            }

            return NOERROR;
        }

        public int GetStreamCaps(int iIndex, IntPtr ppmt, IntPtr pSCC)
        {
            AMMediaType pmt = new AMMediaType(m_mt);

            VideoStreamConfigCaps _caps = new VideoStreamConfigCaps();
            _caps.guid = FormatType.VideoInfo;
            _caps.VideoStandard = AnalogVideoStandard.None;
            _caps.InputSize.Width = m_nWidth;
            _caps.InputSize.Height = m_nHeight;
            _caps.MinCroppingSize.Width = m_nWidth;
            _caps.MinCroppingSize.Height = m_nHeight;

            _caps.MaxCroppingSize.Width = m_nWidth;
            _caps.MaxCroppingSize.Height = m_nHeight;
            _caps.CropGranularityX = m_nWidth;
            _caps.CropGranularityY = m_nHeight;
            _caps.CropAlignX = 0;
            _caps.CropAlignY = 0;

            _caps.MinOutputSize.Width = _caps.MinCroppingSize.Width;
            _caps.MinOutputSize.Height = _caps.MinCroppingSize.Height;
            _caps.MaxOutputSize.Width = _caps.MaxCroppingSize.Width;
            _caps.MaxOutputSize.Height = _caps.MaxCroppingSize.Height;
            _caps.OutputGranularityX = _caps.CropGranularityX;
            _caps.OutputGranularityY = _caps.CropGranularityY;
            _caps.StretchTapsX = 0;
            _caps.StretchTapsY = 0;
            _caps.ShrinkTapsX = 0;
            _caps.ShrinkTapsY = 0;
            _caps.MinFrameInterval = m_nAvgTimePerFrame;
            _caps.MaxFrameInterval = m_nAvgTimePerFrame;
            _caps.MinBitsPerSecond = (_caps.MinOutputSize.Width * _caps.MinOutputSize.Height * 32) * (int)m_nAvgTimePerFrame;
            _caps.MaxBitsPerSecond = (_caps.MaxOutputSize.Width * _caps.MaxOutputSize.Height * 32) * (int)m_nAvgTimePerFrame;


            if (ppmt != IntPtr.Zero)
            {
                IntPtr _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(pmt));
                Marshal.StructureToPtr(pmt, _ptr, true);
                Marshal.WriteIntPtr(ppmt, _ptr);
            }

            if (pSCC != IntPtr.Zero)
            {
                Marshal.StructureToPtr(_caps, pSCC, false);
            }

            return NOERROR;
        }
    }
}
