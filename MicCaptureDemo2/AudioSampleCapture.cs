using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Text.RegularExpressions;

namespace MicCaptureDemo2
{
    /// <summary>
    /// 音频录制，重采样Demo
    /// </summary>
    internal class AudioSampleCapture
    {
        private readonly WasapiCapture _wasapiCapture;
        private readonly IWaveProvider _waveProvider;

        public AudioSampleCapture()
        {
            var wasapiCapture = new WasapiCapture();
            var bufferedProvider = new BufferedWaveProvider(wasapiCapture.WaveFormat)
            {
                DiscardOnBufferOverflow = true,
                ReadFully = false
            };
            wasapiCapture.DataAvailable += (s, e) =>
            {
                bufferedProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            };
            _wasapiCapture = wasapiCapture;
            //采样
            var sampleProvider = bufferedProvider.ToSampleProvider();
            var simpleFormat = ToSimpleFormat(wasapiCapture.WaveFormat);
            // Mono to Stereo
            if (simpleFormat.Channels == 1) sampleProvider = sampleProvider.ToStereo();
            _waveProvider = sampleProvider.ToWaveProvider16();
            //默认音频格式
            TargetFormat = wasapiCapture.WaveFormat;
        }
        /// <summary>
        /// 音频格式
        /// </summary>
        public WaveFormat TargetFormat { get; }
        public void Save()
        {
            using var writer = new WaveFileWriter("recordedAudio.wav", TargetFormat);
            // 将重采样后的数据写入文件
            byte[] buffer = new byte[TargetFormat.AverageBytesPerSecond];
            int bytesRead;
            while ((bytesRead = _waveProvider.Read(buffer, 0, buffer.Length)) > 0)
            {
                writer.Write(buffer, 0, bytesRead);
            }
        }
        private WaveFormat ToSimpleFormat(WaveFormat waveFormat)
        {
            return waveFormat.Encoding == WaveFormatEncoding.IeeeFloat
                ? WaveFormat.CreateIeeeFloatWaveFormat(waveFormat.SampleRate, waveFormat.Channels)
                : new WaveFormat(waveFormat.SampleRate, waveFormat.BitsPerSample, waveFormat.Channels);
        }

        public void Start()
        {
            _wasapiCapture.StartRecording();
        }
        public void Stop()
        {
            _wasapiCapture.StopRecording();
        }
    }
}
