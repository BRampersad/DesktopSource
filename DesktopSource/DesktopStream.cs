using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using DirectShow;
using DirectShow.BaseClasses;
using Sonic;

namespace DesktopSource
{
    // Output Pin (STreams the desktop frames)
    [ComVisible(true)]
    public class DesktopStream : SourceStream, IAMStreamConfig
    {
        public DesktopStream(string _name, BaseSourceFilter _filter) : base(_name, _filter) { }

        public override int GetMediaType(ref AMMediaType pMediaType)
        {
            DesktopSource desktopSource = m_Filter as DesktopSource;
            if (desktopSource != null)
                return desktopSource.GetMediaType(ref pMediaType);

            return E_POINTER;
        }

        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            if (!IsConnected) return VFW_E_NOT_CONNECTED;

            DesktopSource desktopSource = m_Filter as DesktopSource;
            if (desktopSource != null)
                return desktopSource.DecideBufferSize(ref pAlloc, ref prop);

            return E_POINTER;
        }

        public override int FillBuffer(ref IMediaSampleImpl _sample)
        {
            DesktopSource desktopSource = m_Filter as DesktopSource;
            if (desktopSource != null)
                return desktopSource.FillBuffer(ref _sample);

            return E_POINTER;
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
            _caps.InputSize.Width = (m_Filter as DesktopSource).m_nWidth;
            _caps.InputSize.Height = (m_Filter as DesktopSource).m_nHeight;
            _caps.MinCroppingSize.Width = (m_Filter as DesktopSource).m_nWidth;
            _caps.MinCroppingSize.Height = (m_Filter as DesktopSource).m_nHeight;

            _caps.MaxCroppingSize.Width = (m_Filter as DesktopSource).m_nWidth;
            _caps.MaxCroppingSize.Height = (m_Filter as DesktopSource).m_nHeight;
            _caps.CropGranularityX = (m_Filter as DesktopSource).m_nWidth;
            _caps.CropGranularityY = (m_Filter as DesktopSource).m_nHeight;
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
            _caps.MinFrameInterval = (m_Filter as DesktopSource).m_nAvgTimePerFrame;
            _caps.MaxFrameInterval = (m_Filter as DesktopSource).m_nAvgTimePerFrame;
            _caps.MinBitsPerSecond = (_caps.MinOutputSize.Width * _caps.MinOutputSize.Height * 32) * (int)(m_Filter as DesktopSource).m_nAvgTimePerFrame;
            _caps.MaxBitsPerSecond = (_caps.MaxOutputSize.Width * _caps.MaxOutputSize.Height * 32) * (int)(m_Filter as DesktopSource).m_nAvgTimePerFrame;


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
