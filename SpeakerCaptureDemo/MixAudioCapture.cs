using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SpeakerCaptureDemo
{
    /// <summary>
    /// 音频录制，重采样Demo
    /// </summary>
    internal class MixAudioCapture
    {
        private readonly IWaveIn[] _audioWaveCaptures;
        private readonly IWaveProvider _waveProvider;

    public MixAudioCapture(params IWaveIn[] audioWaveCaptures)
    {
        _audioWaveCaptures = audioWaveCaptures;
        var sampleProviders = new List<ISampleProvider>();
        foreach (var waveIn in audioWaveCaptures)
        {
            var bufferedProvider = new BufferedWaveProvider(waveIn.WaveFormat)
            {
                DiscardOnBufferOverflow = true,
                ReadFully = false
            };
            waveIn.DataAvailable += (s, e) =>
            {
                bufferedProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            };
            var sampleProvider = bufferedProvider.ToSampleProvider();
            sampleProviders.Add(sampleProvider);
        }
        var waveProviders = sampleProviders.Select(m => m.ToWaveProvider());
        // 混音重采样后的音频数据
        _waveProvider = new MixingWaveProvider32(waveProviders).ToSampleProvider().ToWaveProvider16();
    }

        /// <summary>
        /// 音频格式
        /// </summary>
        public WaveFormat TargetFormat { get; } = new WaveFormat();
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
        public void Start()
        {
            foreach (var waveIn in _audioWaveCaptures)
            {
                waveIn.StartRecording();
            }
        }
        public void Stop()
        {
            foreach (var waveIn in _audioWaveCaptures)
            {
                waveIn.StopRecording();
            }
        }
    }
}
