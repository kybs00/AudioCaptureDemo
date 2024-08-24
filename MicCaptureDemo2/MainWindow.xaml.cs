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
            _writer.Write(a.Buffer, 0, a.BytesRecorded);
        };
        // 列出所有可用的录音设备
        for (int i = 0; i < WaveIn.DeviceCount; i++)
        {
            var deviceInfo = WaveIn.GetCapabilities(i);
            OutputTextBlock.Text += $"Device {i}: {deviceInfo.ProductName}\r\n";
        }
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