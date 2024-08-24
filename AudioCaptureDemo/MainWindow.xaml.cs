using System.Diagnostics;
using System.Windows;
using NAudio.Wave;

namespace MicCaptureDemo1
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
    private WaveInEvent _waveIn;
    private WaveFileWriter _writer;
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var waveIn = new WaveIn();
        _waveIn = new WaveInEvent();
        //441采样率，单通道
        _waveIn.WaveFormat = new WaveFormat(44100, 1);
        _writer = new WaveFileWriter("recordedAudio.wav", _waveIn.WaveFormat);
        _waveIn.DataAvailable += (s, a) =>
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
        _waveIn.StartRecording();
    }
    private void StopRecordButton_OnClick(object sender, RoutedEventArgs e)
    {
        _waveIn.StopRecording();
        _waveIn.Dispose();
        _writer.Close();
    }
    }
}