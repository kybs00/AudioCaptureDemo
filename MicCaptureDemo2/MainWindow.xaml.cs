using System.Diagnostics;
using System.Windows;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicCaptureDemo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private WaveFileWriter _writer;
        private WasapiCapture _capture;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _capture = new WasapiCapture();
            _writer = new WaveFileWriter("recordedAudio.wav", _capture.WaveFormat);
            _capture.DataAvailable += (s, a) =>
            {
                GetWavePoints(a.Buffer, a.BytesRecorded);
                _writer.Write(a.Buffer, 0, a.BytesRecorded);
            };
            // 列出所有可用的录音设备
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var deviceInfo = WaveIn.GetCapabilities(i);
                OutputTextBlock.Text += $"Device {i}: {deviceInfo.ProductName}\r\n";
            }
        }
        private List<Point> GetWavePoints(byte[] buffer, int bytesRecorded)
        {
            var points = new List<Point>();
            if (bytesRecorded == 0)
            {
                return points;
            }
            var waveform = new float[bytesRecorded / 2];
            for (int i = 0; i < bytesRecorded; i += 2)
            {
                // 将字节对转换为16位PCM样本
                waveform[i / 2] = (short)(buffer[i] | (buffer[i + 1] << 8)) / 32768f;
            }

            for (int i = 0; i < waveform.Length; i++)
            {
                points.Add(new Point(i, waveform[i]));
            }
            return points;
        }
        private void StartRecordButton_OnClick(object sender, RoutedEventArgs e)
        {
            _capture.StartRecording();
        }
        private void StopRecordButton_OnClick(object sender, RoutedEventArgs e)
        {
            _capture.StopRecording();
            _capture.Dispose();
            _writer.Close();
        }
    }
}